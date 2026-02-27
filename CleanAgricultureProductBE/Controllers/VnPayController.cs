using CleanAgricultureProductBE.Repositories.Order;
using CleanAgricultureProductBE.Services.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VNPAY;
using VNPAY.Models.Exceptions;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/v1/vnpay")]
    [ApiController]
    public class VnPayController : ControllerBase
    {
        private readonly IVnpayClient _vnpayClient;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<TestPaymentController> _logger;

        public VnPayController(IVnpayClient vnpayClient, IPaymentService paymentService, ILogger<TestPaymentController> logger)
        {
            _vnpayClient = vnpayClient;
            _paymentService = paymentService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("ProceedAfterPayment")]
        public async Task<IActionResult> ProceedAfterPayment()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Forbid("Authenticated users are not allowed to access this endpoint. It is intended for VNPAY's server to call after payment processing.");
            }

            try
            {
                var paymentResult = _vnpayClient.GetPaymentResult(this.Request);

                // Ghi log thông tin thanh toán để tiện theo dõi nếu cần
                _logger.LogInformation("Call At VnpayController!");
                _logger.LogInformation("Payment ID: {PaymentId}", paymentResult.PaymentId);
                _logger.LogInformation("VNPAY Transaction ID: {VnpayTransactionId}", paymentResult.VnpayTransactionId);
                _logger.LogInformation("Timestamp: {Timestamp}", paymentResult.Timestamp);
                _logger.LogInformation("Card Type: {CardType}", paymentResult.CardType);
                _logger.LogInformation("Banking Info: {BankingInfo}", paymentResult.BankingInfor != null ? $"{paymentResult.BankingInfor.BankCode} - {paymentResult.BankingInfor.BankTransactionId}" : "N/A");
                _logger.LogInformation("Desc:" + paymentResult.Description);
                // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.

                await _paymentService.HandlePayment(paymentResult.Description, paymentResult.VnpayTransactionId+"");

                return Ok();
            }
            catch (VnpayException ex)  // Bắt lỗi liên quan đến VNPAY
            {
                _logger.LogError(ex, "VNPAY Error: {Message}, TransactionStatusCode: {TransactionStatusCode}, PaymentResponseCode: {PaymentResponseCode}", ex.Message, ex.TransactionStatusCode, ex.PaymentResponseCode);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing payment");
                return BadRequest(ex.Message);
            }
        }
    }
}
