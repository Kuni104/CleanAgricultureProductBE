namespace CleanAgricultureProductBE.Repositories.Payment
{
    public interface IPaymentRepository
    {
        public Task AddPayment(Models.Payment payment);

        public Task UpdatePayment(Models.Payment payment);
        public Task<Models.Payment?> GetPaymentById(Guid paymentId);
    }
}
