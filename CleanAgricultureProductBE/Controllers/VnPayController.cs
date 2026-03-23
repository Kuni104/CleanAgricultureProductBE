using CleanAgricultureProductBE.Services.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
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
        private readonly ILogger<VnPayController> _logger;
        private readonly string _frontendReturnUrl;

        public VnPayController(
            IVnpayClient vnpayClient,
            IPaymentService paymentService,
            IConfiguration configuration,
            ILogger<VnPayController> logger)
        {
            _vnpayClient = vnpayClient;
            _paymentService = paymentService;
            _logger = logger;
            _frontendReturnUrl = configuration["VNPAY:FrontendReturnUrl"] ?? "http://localhost:5173/checkout";
        }

        [AllowAnonymous]
        [HttpGet("callback")]
        [SwaggerOperation(Summary = "Nhan ket qua thanh toan tu VNPay va redirect nguoi dung ve frontend")]
        public async Task<IActionResult> Callback()
        {
            var orderId = Request.Query["vnp_OrderInfo"].ToString();
            var transactionCode = Request.Query["vnp_TransactionNo"].ToString();
            var isSuccess = IsSuccessfulPayment();

            if (Guid.TryParse(orderId, out _))
            {
                try
                {
                    await _paymentService.HandlePaymentResult(orderId, transactionCode, isSuccess);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to handle VNPay callback for order {OrderId}", orderId);
                }
            }

            return Redirect(BuildFrontendRedirectUrl(isSuccess));
        }

        [AllowAnonymous]
        [HttpGet("ipn")]
        [HttpGet("proceed-after-payment")]
        [SwaggerOperation(Summary = "IPN xu ly ket qua thanh toan tu VNPay")]
        public async Task<IActionResult> ProceedAfterPayment()
        {
            try
            {
                var paymentResult = _vnpayClient.GetPaymentResult(Request);
                var isSuccess = IsSuccessfulPayment();

                _logger.LogInformation("Call At VnpayController!");
                _logger.LogInformation("Payment ID: {PaymentId}", paymentResult.PaymentId);
                _logger.LogInformation("VNPAY Transaction ID: {VnpayTransactionId}", paymentResult.VnpayTransactionId);
                _logger.LogInformation("Timestamp: {Timestamp}", paymentResult.Timestamp);
                _logger.LogInformation("Card Type: {CardType}", paymentResult.CardType);
                _logger.LogInformation(
                    "Banking Info: {BankingInfo}",
                    paymentResult.BankingInfor != null
                        ? $"{paymentResult.BankingInfor.BankCode} - {paymentResult.BankingInfor.BankTransactionId}"
                        : "N/A");
                _logger.LogInformation("Desc:{Description}", paymentResult.Description);

                await _paymentService.HandlePaymentResult(
                    paymentResult.Description,
                    paymentResult.VnpayTransactionId + "",
                    isSuccess);

                return Ok();
            }
            catch (VnpayException ex)
            {
                _logger.LogError(
                    ex,
                    "VNPAY Error: {Message}, TransactionStatusCode: {TransactionStatusCode}, PaymentResponseCode: {PaymentResponseCode}",
                    ex.Message,
                    ex.TransactionStatusCode,
                    ex.PaymentResponseCode);
                await TryHandleFailedPaymentFromQuery();
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing payment");
                await TryHandleFailedPaymentFromQuery();
                return BadRequest(ex.Message);
            }
        }

        private bool IsSuccessfulPayment()
        {
            var responseCode = Request.Query["vnp_ResponseCode"].ToString();
            var transactionStatus = Request.Query["vnp_TransactionStatus"].ToString();

            return responseCode == "00" &&
                   (string.IsNullOrWhiteSpace(transactionStatus) || transactionStatus == "00");
        }

        private string BuildFrontendRedirectUrl(bool isSuccess)
        {
            var parameters = new Dictionary<string, string?>();
            foreach (var item in Request.Query)
            {
                parameters[item.Key] = item.Value.ToString();
            }

            parameters["paymentStatus"] = isSuccess ? "success" : "failed";

            return QueryHelpers.AddQueryString(_frontendReturnUrl, parameters);
        }

        private async Task TryHandleFailedPaymentFromQuery()
        {
            var orderId = Request.Query["vnp_OrderInfo"].ToString();
            var transactionCode = Request.Query["vnp_TransactionNo"].ToString();

            if (!Guid.TryParse(orderId, out _))
            {
                return;
            }

            try
            {
                await _paymentService.HandlePaymentResult(orderId, transactionCode, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark VNPay payment as failed for order {OrderId}", orderId);
            }
        }
    }
}
