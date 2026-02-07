using CleanAgricultureProductBE.DTOs.PaymentMethod;
using CleanAgricultureProductBE.Repositories.PaymentMethod;

namespace CleanAgricultureProductBE.Services.PaymentMethod
{
    public class PaymentMethodService(IPaymentMethodRepository paymentMethodRepository) : IPaymentMethodService
    {
        public async Task<PaymentMethodResponseDto> CreatePaymentMethod(PaymentMethodRequestDto request)
        {
            var paymentExists = await paymentMethodRepository.CheckExistedPaymentMethodByName(request.MethodName);
            if (paymentExists)
            {
                return null!;
            }

            var newPaymentMethod = new Models.PaymentMethod
            {   
                MethodName = request.MethodName
            };
            
            await paymentMethodRepository.CreatePaymentMethod(newPaymentMethod);

            var result = new PaymentMethodResponseDto
            {
                PaymentMethodId = newPaymentMethod.PaymentMethodId,
                MethodName = newPaymentMethod.MethodName
            };

            return result;
        }

        public async Task<List<PaymentMethodResponseDto>> GetPaymentMethods()
        {
            var paymentMethodList = await paymentMethodRepository.GetAllPaymentMethods();

            var result = paymentMethodList
                .Select(pm => new PaymentMethodResponseDto
                {
                    PaymentMethodId = pm.PaymentMethodId,
                    MethodName = pm.MethodName
                })
                .ToList();

            return result;
        }
    }
}
