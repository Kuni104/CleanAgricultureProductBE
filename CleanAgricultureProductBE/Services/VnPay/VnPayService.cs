using Microsoft.AspNetCore.Mvc;
using VNPAY;
using VNPAY.Models;

namespace CleanAgricultureProductBE.Services.VnPay
{
    public class VnPayService : IVnPayService
    {
        private readonly IVnpayClient _vnpayClient;

        public VnPayService(IVnpayClient vnpayClient)
        {
            _vnpayClient = vnpayClient;
        }

        public string CreatePaymentUrl(VnpayPaymentRequest request)
        {
            try
            {
                var paymentUrlInfo = _vnpayClient.CreatePaymentUrl(request);

                return paymentUrlInfo.Url;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Khong the tao duong dan thanh toan VNPay.", ex);
            }
        }
    }
}
