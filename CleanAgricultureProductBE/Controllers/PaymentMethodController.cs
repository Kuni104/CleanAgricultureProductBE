using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.PaymentMethod;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services.PaymentMethod;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanAgricultureProductBE.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Route("api/v1/payment-method")]
    [ApiController]
    public class PaymentMethodController(IPaymentMethodService paymentMethodService) : ControllerBase
    {
        [HttpGet] 
        [SwaggerOperation(Summary = "Lấy danh sách phương thức thanh toán (Admin/Staff)")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var success = "true";
            var message = "";

            var result = await paymentMethodService.GetPaymentMethods();

            if (result != null) {
                message = "Lấy các hình thức thanh toán thành công";
            }
            else
            {
                message = "Không có hình thức thanh toán nào";
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
        [SwaggerOperation(Summary = "Tạo phương thức thanh toán mới (Admin/Staff)")]
        public async Task<IActionResult> CreatePaymentMethod([FromBody]PaymentMethodRequestDto request)
        {
            var success = "";
            var message = "";

            var result = await paymentMethodService.CreatePaymentMethod(request);

            if (result != null)
            {
                success = "true";
                message = "Tạo hình thức thanh toán thành công";
            }
            else
            {
                success = "false";
                message = "Hình thức thanh toán đã tồn tại";
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
        [SwaggerOperation(Summary = "Cập nhật phương thức thanh toán (Admin/Staff)")]
        public async Task<IActionResult> UpdatePaymentMethod(int paymentMethodId, [FromBody]PaymentMethodRequestDto request)
        {
            var success = "";
            var message = "";

            var result = await paymentMethodService.UpdatePaymentMethod(paymentMethodId, request);
            if (result != null)
            {
                success = "true";
                message = "Cập nhật hình thức thánh toán thành công";
            }
            else
            {
                success = "false";
                message = "Không tìm thấy hình thức thanh toán hoặc hình thức thanh toán muốn cập nhật đã tồn tại";
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
        [SwaggerOperation(Summary = "Xóa phương thức thanh toán (Admin/Staff)")]
        public async Task<IActionResult> DeletePaymentMethod(int paymentMethodId)
        {
            var success = "";
            var message = "";

            var result = await paymentMethodService.DeletePaymentMethod(paymentMethodId);
            if (result)
            {
                success = "true";
                message = "Xóa hình thức thánh toán thành công";
            }
            else
            {
                success = "false";
                message = "Không có hình thức thanh toán";
            }

            var response = new ResponseObject<string>()
            {
                Success = success,
                Message = message,
                Data = result ? "Đã xóa" : "Chưa Xóa"
            };

            return Ok(response);
        }
    }
}
