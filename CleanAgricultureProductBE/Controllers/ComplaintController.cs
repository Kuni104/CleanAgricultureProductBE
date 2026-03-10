using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Complaint;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services.Complaint;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class ComplaintController(IComplaintService complaintService) : ControllerBase
    {
        // Customer: tạo complaint cho order (có thể kèm product)
        [Authorize(Roles = "Customer")]
        [HttpPost("me/complaints")]
        [Consumes("application/json")]
        [SwaggerOperation(Summary = "Tạo khiếu nại cho đơn hàng (Customer)")]
        public async Task<IActionResult> CreateComplaint([FromBody] CreateComplaintRequestDto request)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)!;
            try
            {
                var result = await complaintService.CreateComplaintAsync(email, request);
                return base.Ok(new DTOs.ApiResponse.ResponseObject<ComplaintResponseDto>
                {
                    Success = "true",
                    Message = "Gửi khiếu nại thành công",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return base.Conflict(new DTOs.ApiResponse.ResponseObject<object> { Success = "false", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return base.BadRequest(new DTOs.ApiResponse.ResponseObject<object> { Success = "false", Message = ex.Message });
            }
        }

        // Customer: xem danh sách complaint của mình
        [Authorize(Roles = "Customer")]
        [HttpGet("me/complaints")]
        [SwaggerOperation(Summary = "Lấy danh sách khiếu nại của tôi (Customer)")]
        public async Task<IActionResult> GetMyComplaints([FromQuery] int? page, [FromQuery] int? size)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)!;
            var result = await complaintService.GetMyComplaintsAsync(email, page, size);
            return base.Ok(new ResponseObjectWithPagination<List<ComplaintResponseDto>>
            {
                Success = "true",
                Message = result.ResultObject?.Count == 0 ? "Không có khiếu nại nào" : "Lấy các khiếu nại thành công",
                Data = result.ResultObject,
                Pagination = result.Pagination
            });
        }

        // Staff: xem tất cả complaints
        [Authorize(Roles = "Staff,Admin")]
        [HttpGet("complaints")]
        [SwaggerOperation(Summary = "Lấy tất cả khiếu nại (Staff/Admin)")]
        public async Task<IActionResult> GetAllComplaints([FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? keyword)
        {
            var result = await complaintService.GetAllComplaintsAsync(page, size, keyword);
            return base.Ok(new ResponseObjectWithPagination<List<ComplaintResponseDto>>
            {
                Success = "true",
                Message = result.ResultObject?.Count == 0 ? "Không có khiếu nại nào" : "Lấy các khiếu nại thành công",
                Data = result.ResultObject,
                Pagination = result.Pagination
            });
        }

        // Staff: xem chi tiết 1 complaint
        [Authorize(Roles = "Staff,Admin")]
        [HttpGet("complaints/{complaintId}")]
        [SwaggerOperation(Summary = "Lấy chi tiết khiếu nại theo ID (Staff/Admin)")]
        public async Task<IActionResult> GetComplaintById([FromRoute] Guid complaintId)
        {
            var result = await complaintService.GetComplaintByIdAsync(complaintId);
            if (result == null)
                return base.NotFound(new DTOs.ApiResponse.ResponseObject<object> { Success = "false", Message = "Không tìm thấy khiếu nại" });

            return base.Ok(new DTOs.ApiResponse.ResponseObject<ComplaintResponseDto>
            {
                Success = "true",
                Message = "Lấy thông tin khiếu nại thành công",
                Data = result
            });
        }

        // Staff: cập nhật trạng thái complaint (Resolved + Exchange/Refund, hoặc Rejected)
        [Authorize(Roles = "Staff,Admin")]
        [HttpPatch("complaints/{complaintId}/status")]
        [SwaggerOperation(Summary = "Cập nhật trạng thái khiếu nại - Resolved (Exchange/Refund) hoặc Rejected (Staff/Admin)")]
        public async Task<IActionResult> UpdateComplaintStatus([FromRoute] Guid complaintId, [FromBody] UpdateComplaintStatusDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)!;
            try
            {
                var result = await complaintService.UpdateComplaintStatusAsync(email, complaintId, dto);
                var message = dto.Status == "Resolved"
                    ? $"Khiếu nại đã được xử lý: {dto.Resolution}"
                    : "Khiếu nại đã bị từ chối";
                return base.Ok(new DTOs.ApiResponse.ResponseObject<ComplaintResponseDto>
                {
                    Success = "true",
                    Message = message,
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                return base.Conflict(new DTOs.ApiResponse.ResponseObject<object> { Success = "false", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return base.BadRequest(new DTOs.ApiResponse.ResponseObject<object> { Success = "false", Message = ex.Message });
            }
        }
    }
}
