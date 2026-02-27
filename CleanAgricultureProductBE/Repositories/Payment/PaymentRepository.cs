
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

        public async Task UpdatePayment(Models.Payment payment)
        {
            context.Payments.Update(payment);
            await context.SaveChangesAsync();
        }

        public async Task<Models.Payment?> GetPaymentById(Guid paymentId)
        {
            return await context.Payments.FindAsync(paymentId);
        }
    }
}
