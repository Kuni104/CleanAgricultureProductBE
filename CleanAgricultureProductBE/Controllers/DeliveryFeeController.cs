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
                message = "No delivery fee data found";
            }
            else
            {
                success = "true";
                message = "Delivery fee data retrieved successfully";
            }

            var response = new ResponseObject<List<DeliveryFeeResponseDto>>
            {
                Success = success,
                Message = message,
                Data = result
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
                message = "Failed to add delivery fee data";
            }
            else
            {
                success = "true";
                message = "Delivery fee data added successfully";
            }

            var response = new ResponseObject<DeliveryFeeResponseDto>
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
                return BadRequest("No Existed Delivery Fee With This ID");
            }

            var response = new ResponseObject<DeliveryFeeResponseDto>
            {
                Success = "true",
                Message = "Delivery Fee Updated Successfully",
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
                return BadRequest("No Existed Delivery Fee With This ID");
            }

            var response = new ResponseObject<bool>
            {
                Success = "true",
                Message = "Delivery Fee Deleted Successfully",
                Data = result
            };
            return Ok(response);
        }
    }
}
