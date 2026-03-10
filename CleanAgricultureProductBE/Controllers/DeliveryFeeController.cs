using Azure.Core;
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.DeliveryFee;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services.DeliveryFee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanAgricultureProductBE.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/v1/delivery-fee")]
    [ApiController]
    public class DeliveryFeeController(IDeliveryFeeService deliveryFeeService) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách phí giao hàng (Admin)")]
        public async Task<IActionResult> GetDeliveryFeeList()
        {
            var success = "";
            var message = "";

            var result = await deliveryFeeService.GetDeliveryFeeList();

            if (result == null || result.Count == 0)
            {
                success = "false";
                message = "Không có phí giao hàng nào";
            }
            else
            {
                success = "true";
                message = "Lấy các phí giao hàng thành công";
            }

            var response = new ResponseObjectWithPagination<List<DeliveryFeeResponseDto>>
            {
                Success = success,
                Message = message,
                Data = result,
                Pagination = null
            };

            return Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Thêm phí giao hàng mới (Admin)")]
        public async Task<IActionResult> AddDeliveryFee([FromBody] CreateDeliveryFeeRequestDto request)
        {
            if (string.IsNullOrEmpty(request.City.Trim()) || string.IsNullOrEmpty(request.Ward.Trim()) || string.IsNullOrEmpty(request.District.Trim()))
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Không được để trống thành phố, xã, phường"
                });
            }

            var success = "";
            var message = "";

            var result = await deliveryFeeService.AddDeliveryFee(request);

            if (result == null)
            {
                success = "false";
                message = "Đã có phí giao hàng với thông tin này";
            }
            else
            {
                success = "true";
                message = "Thêm phí giao hàng thành công";
            }

            var response = new ResponseObject<DeliveryFeeResponseDto>
            {
                Success = success,
                Message = message,
                Data = result
            };

            return Ok(response);
        }

        [HttpPatch("{deliveryFeeId}")]
        [SwaggerOperation(Summary = "Cập nhật phí giao hàng (Admin)")]
        public async Task<IActionResult> UpdateDeliveryFee([FromRoute]Guid deliveryFeeId, [FromBody]UpdateDeliveryFeeRequestDto request)
        {
            var result = await deliveryFeeService.UpdateDeliveryFee(deliveryFeeId, request);
            if (result == null)
            {
                return base.BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = $"Không có phí giao hàng nào với ID:{deliveryFeeId}"
                });
            }else if(result.Status == "Invalid Request")
            {
                return base.BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Request không hợp lệ (có thể km nằm giữa km đã có)"
                });
            }

            var response = new ResponseObject<DeliveryFeeResponseDto>
            {
                Success = "true",
                Message = "Cập nhật phí giao hàng thành công",
                Data = result.Data
            };

            return Ok(response);
        }

        [HttpDelete("{deliveryFeeId}")]
        [SwaggerOperation(Summary = "Xóa phí giao hàng (Admin)")]
        public async Task<IActionResult> DeleteDeliveryFeeById([FromRoute]Guid deliveryFeeId)
        {
            var result = await deliveryFeeService.DeleteDeliveryFeeById(deliveryFeeId);
            if (!result)
            {
                return base.BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = $"Không có phí giao hàng nào với ID:{deliveryFeeId}"
                });
            }

            var response = new ResponseObject<bool>
            {
                Success = "true",
                Message = "Xóa phí giao hàng thành công",
                Data = result
            };
            return Ok(response);
        }
    }
}
