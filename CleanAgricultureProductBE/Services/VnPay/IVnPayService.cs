using VNPAY.Models;

namespace CleanAgricultureProductBE.Services.VnPay
{
    public interface IVnPayService
    {
        public string CreatePaymentUrl(VnpayPaymentRequest request);
    }
}
