using CleanAgricultureProductBE.DTOs.Complaint;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services.Complaint;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api")]
    [ApiController]
    public class ComplaintController(IComplaintService complaintService) : ControllerBase
    {
        // Customer: tạo complaint cho order (có thể kèm product)
        [Authorize(Roles = "Customer")]
        [HttpPost("me/complaints")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateComplaint([FromBody] CreateComplaintRequestDto request)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)!;
            try
            {
                var result = await complaintService.CreateComplaintAsync(email, request);
                return Ok(new ResponseObject<ComplaintResponseDto>
                {
                    Success = "true",
                    Message = "Complaint submitted successfully",
                    Data = result
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ResponseObject<object> { Success = "false", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<object> { Success = "false", Message = ex.Message });
            }
        }

        // Customer: xem danh sách complaint của mình
        [Authorize(Roles = "Customer")]
        [HttpGet("me/complaints")]
        public async Task<IActionResult> GetMyComplaints([FromQuery] int? page, [FromQuery] int? size)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)!;
            var result = await complaintService.GetMyComplaintsAsync(email, page, size);
            return Ok(new ResponseObject<List<ComplaintResponseDto>>
            {
                Success = "true",
                Message = result.ResultObject?.Count == 0 ? "No complaints found" : "Complaints retrieved successfully",
                Data = result.ResultObject,
                Pagination = result.Pagination
            });
        }

        // Staff: xem tất cả complaints
        [Authorize(Roles = "Staff,Admin")]
        [HttpGet("complaints")]
        public async Task<IActionResult> GetAllComplaints([FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? keyword)
        {
            var result = await complaintService.GetAllComplaintsAsync(page, size, keyword);
            return Ok(new ResponseObject<List<ComplaintResponseDto>>
            {
                Success = "true",
                Message = result.ResultObject?.Count == 0 ? "No complaints found" : "Complaints retrieved successfully",
                Data = result.ResultObject,
                Pagination = result.Pagination
            });
        }

        // Staff: xem chi tiết 1 complaint
        [Authorize(Roles = "Staff,Admin")]
        [HttpGet("complaints/{complaintId}")]
        public async Task<IActionResult> GetComplaintById([FromRoute] Guid complaintId)
        {
            var result = await complaintService.GetComplaintByIdAsync(complaintId);
            if (result == null)
                return NotFound(new ResponseObject<object> { Success = "false", Message = "Complaint not found" });

            return Ok(new ResponseObject<ComplaintResponseDto>
            {
                Success = "true",
                Message = "Complaint retrieved successfully",
                Data = result
            });
        }
    }
}
