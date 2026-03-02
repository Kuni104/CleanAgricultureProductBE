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
    [Authorize(Roles = "Admin,Staff")]
    [Route("api/v1/delivery-fee")]
    [ApiController]
    public class DeliveryFeeController(IDeliveryFeeService deliveryFeeService) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách phí giao hàng (Admin/Staff)")]
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

            var response = new DTOs.Response.ResponseObject<List<DeliveryFeeResponseDto>>
            {
                Success = success,
                Message = message,
                Data = result,
                Pagination = null
            };

            return Ok(response);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Thêm phí giao hàng mới (Admin/Staff)")]
        public async Task<IActionResult> AddDeliveryFee([FromBody] CreateDeliveryFeeRequestDto request)
        {
            var success = "";
            var message = "";

            var result = await deliveryFeeService.AddDeliveryFee(request);

            if (result == null)
            {
                success = "false";
                message = "Thêm phí giao hàng không thành công";
            }
            else
            {
                success = "true";
                message = "Thêm phí giao hàng thành công";
            }

            var response = new DTOs.ApiResponse.ResponseObject<DeliveryFeeResponseDto>
            {
                Success = success,
                Message = message,
                Data = result
            };

            return Ok(response);
        }

        [HttpPut("{deliveryFeeId}")]
        [SwaggerOperation(Summary = "Cập nhật phí giao hàng (Admin/Staff)")]
        public async Task<IActionResult> UpdateDeliveryFee([FromRoute]Guid deliveryFeeId, [FromBody]UpdateDeliveryFeeRequestDto resquest)
        {
            var result = await deliveryFeeService.UpdateDeliveryFee(deliveryFeeId, resquest);
            if (result == null)
            {
                return base.BadRequest(new DTOs.ApiResponse.ResponseObject<string>
                {
                    Success = "false",
                    Message = $"Không có phí giao hàng nào với ID:{deliveryFeeId}"
                });
            }

            var response = new DTOs.ApiResponse.ResponseObject<DeliveryFeeResponseDto>
            {
                Success = "true",
                Message = "Cập nhật phí giao hàng thành công",
                Data = result
            };

            return Ok(response);
        }

        [HttpDelete("{deliveryFeeId}")]
        [SwaggerOperation(Summary = "Xóa phí giao hàng (Admin/Staff)")]
        public async Task<IActionResult> DeleteDeliveryFeeById([FromRoute]Guid deliveryFeeId)
        {
            var result = await deliveryFeeService.DeleteDeliveryFeeById(deliveryFeeId);
            if (!result)
            {
                return base.BadRequest(new DTOs.ApiResponse.ResponseObject<string>
                {
                    Success = "false",
                    Message = $"Không có phí giao hàng nào với ID:{deliveryFeeId}"
                });
            }

            var response = new DTOs.ApiResponse.ResponseObject<bool>
            {
                Success = "true",
                Message = "Xóa phí giao hàng thành công",
                Data = result
            };
            return Ok(response);
        }
    }
}
