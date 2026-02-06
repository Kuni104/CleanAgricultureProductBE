using CleanAgricultureProductBE.DTOs.DeliveryFee;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services.DeliveryFee;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryFeeController(IDeliveryFeeService deliveryFeeService) : ControllerBase
    {
        [HttpGet]
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

            var response = new ResponseObject<List<GetDeliveryFeeResponseDto>>
            {
                Success = success,
                Message = message,
                Data = result
            };

            return Ok(response);
        }

        [HttpPost]
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

            var response = new ResponseObject<GetDeliveryFeeResponseDto>
            {
                Success = success,
                Message = message,
                Data = result
            };

            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDeliveryFee([FromBody] GetDeliveryFeeResponseDto resquest)
        {
            var result = await deliveryFeeService.UpdateDeliveryFee(resquest);
            if (result == null)
            {
                return BadRequest("No Existed Delivery Fee With This ID");
            }

            var response = new ResponseObject<GetDeliveryFeeResponseDto>
            {
                Success = "true",
                Message = "Delivery Fee Updated Successfully",
                Data = result
            };

            return Ok(response);
        }

        [HttpDelete("{deliveryFeeId}")]
        public async Task<IActionResult> DeleteDeliveryFeeById(Guid deliveryFeeId)
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
