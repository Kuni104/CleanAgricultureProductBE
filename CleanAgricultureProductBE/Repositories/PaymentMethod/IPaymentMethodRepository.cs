
namespace CleanAgricultureProductBE.Repositories.PaymentMethod
{
    public interface IPaymentMethodRepository
    {
        public Task<List<Models.PaymentMethod>> GetAllPaymentMethods();
        public Task CreatePaymentMethod(Models.PaymentMethod newPaymentMethod);
        public Task<bool> CheckExistedPaymentMethodByName(string paymentMethodName);

    }
}
