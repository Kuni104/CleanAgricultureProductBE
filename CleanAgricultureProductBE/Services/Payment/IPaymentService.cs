namespace CleanAgricultureProductBE.Services.Payment
{
    public interface IPaymentService
    {
        public Task CreatePayment();

        public Task HandlePayment(string paymentId, string transactionCode);
    }
}
