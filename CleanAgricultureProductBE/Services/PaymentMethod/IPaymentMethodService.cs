using CleanAgricultureProductBE.DTOs.PaymentMethod;

namespace CleanAgricultureProductBE.Services.PaymentMethod
{
    public interface IPaymentMethodService
    {
        public Task<List<PaymentMethodResponseDto>> GetPaymentMethods();
        public Task<PaymentMethodResponseDto> CreatePaymentMethod(PaymentMethodRequestDto request);
        public Task<PaymentMethodResponseDto> UpdatePaymentMethod(int id, PaymentMethodRequestDto request);
        public Task<bool> DeletePaymentMethod(int paymentMethodId);
    }
}
