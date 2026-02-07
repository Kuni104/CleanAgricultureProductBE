
namespace CleanAgricultureProductBE.Repositories.PaymentMethod
{
    public interface IPaymentMethodRepository
    {
        public Task<Models.PaymentMethod?> GetPaymentMethodById(int paymentMethodId);
        public Task<List<Models.PaymentMethod>> GetAllPaymentMethods();
        public Task CreatePaymentMethod(Models.PaymentMethod newPaymentMethod);
        public Task<bool> CheckExistedPaymentMethodByName(string paymentMethodName);
        public Task UpdatePaymentMethod(Models.PaymentMethod paymentMethod);

    }
}
