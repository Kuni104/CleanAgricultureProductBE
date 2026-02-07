using CleanAgricultureProductBE.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.PaymentMethod
{
    public class PaymentMethodRepository(AppDbContext context) : IPaymentMethodRepository
    {
        public async Task<Models.PaymentMethod?> GetPaymentMethodById(int paymentMethodId)
        {
            return await context.PaymentMethods.FindAsync(paymentMethodId);
        }
        public async Task<List<Models.PaymentMethod>> GetAllPaymentMethods()
        {
            return await context.PaymentMethods.ToListAsync();
        }

        public async Task CreatePaymentMethod(Models.PaymentMethod newPaymentMethod)
        {
            context.PaymentMethods.Add(newPaymentMethod);
            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckExistedPaymentMethodByName(string paymentMethodName)
        {
            return await context.PaymentMethods.AnyAsync(pm => pm.MethodName.ToLower() == paymentMethodName.ToLower());
        }

        public async Task UpdatePaymentMethod(Models.PaymentMethod paymentMethod)
        {
            context.PaymentMethods.Update(paymentMethod);
            await context.SaveChangesAsync();
        }

        public async Task DeletePaymentMethod(Models.PaymentMethod paymentMethod)
        {
            context.PaymentMethods.Remove(paymentMethod);
            await context.SaveChangesAsync();
        }
    }
}
