namespace CleanAgricultureProductBE.Services.Payment
{
    public interface IPaymentService
    {
        public Task CreatePayment();

        public Task HandlePaymentResult(string orderId, string? transactionCode, bool isSuccess);
    }
}
