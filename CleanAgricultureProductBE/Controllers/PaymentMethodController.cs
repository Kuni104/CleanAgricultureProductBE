using CleanAgricultureProductBE.DTOs.PaymentMethod;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services.PaymentMethod;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanAgricultureProductBE.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Route("api/payment-method")]
    [ApiController]
    public class PaymentMethodController(IPaymentMethodService paymentMethodService) : ControllerBase
    {
        [HttpGet] 
        public async Task<IActionResult> GetPaymentMethods()
        {
            var success = "true";
            var message = "";

            var result = await paymentMethodService.GetPaymentMethods();

            if (result != null) {
                message = "Payment methods retrieved successfully";
            }
            else
            {
                message = "No payment methods found";
            }

            var response = new ResponseObject<List<PaymentMethodResponseDto>>
            {
                Success = success,
                Message = message,
                Data = result
            };

            return Ok(response);
        }

        [HttpPost] 
        public async Task<IActionResult> CreatePaymentMethod([FromBody]PaymentMethodRequestDto request)
        {
            var success = "";
            var message = "";

            var result = await paymentMethodService.CreatePaymentMethod(request);

            if (result != null)
            {
                success = "true";
                message = "Payment method created successfully";
            }
            else
            {
                success = "false";
                message = "Failed to create payment method | Already existed payment method!";
            }

            var response = new ResponseObject<PaymentMethodResponseDto>()
            {
                Success = success,
                Message = message,
                Data = result
            };

            return result != null ? Ok(response) : BadRequest(response);
        }

        [HttpPut("{paymentMethodId}")] 
        public async Task<IActionResult> UpdatePaymentMethod(int paymentMethodId, [FromBody]PaymentMethodRequestDto request)
        {
            var success = "";
            var message = "";

            var result = await paymentMethodService.UpdatePaymentMethod(paymentMethodId, request);
            if (result != null)
            {
                success = "true";
                message = "Payment method updated successfully";
            }
            else
            {
                success = "false";
                message = "Failed to update payment method | Payment method not found or already existed payment method!";
            }

            var response = new ResponseObject<PaymentMethodResponseDto>()
            {
                Success = success,
                Message = message,
                Data = result
            };

            return Ok(response);
        }

        [HttpDelete("{paymentMethodId}")]
        public async Task<IActionResult> DeletePaymentMethod(int paymentMethodId)
        {
            var success = "";
            var message = "";

            var result = await paymentMethodService.DeletePaymentMethod(paymentMethodId);
            if (result)
            {
                success = "true";
                message = "Payment method deleted successfully";
            }
            else
            {
                success = "false";
                message = "Failed to delete payment method | Payment method not found!";
            }

            var response = new ResponseObject<string>()
            {
                Success = success,
                Message = message,
                Data = result ? "Deleted" : "Not Deleted"
            };

            return Ok(response);
        }
    }
}
