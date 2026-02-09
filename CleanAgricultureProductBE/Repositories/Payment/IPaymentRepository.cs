namespace CleanAgricultureProductBE.Repositories.Payment
{
    public interface IPaymentRepository
    {
        public Task AddPayment(Models.Payment payment);
    }
}
