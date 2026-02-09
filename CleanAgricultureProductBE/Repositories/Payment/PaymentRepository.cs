
using CleanAgricultureProductBE.Data;

namespace CleanAgricultureProductBE.Repositories.Payment
{
    public class PaymentRepository(AppDbContext context) : IPaymentRepository
    {
        public async Task AddPayment(Models.Payment payment)
        {
            context.Payments.Add(payment);
            await context.SaveChangesAsync();
        }
    }
}
