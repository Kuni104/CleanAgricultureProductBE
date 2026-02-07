using CleanAgricultureProductBE.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.PaymentMethod
{
    public class PaymentMethodRepository(AppDbContext context) : IPaymentMethodRepository
    {
        public async Task<List<Models.PaymentMethod>> GetAllPaymentMethods()
        {
            return await context.Set<Models.PaymentMethod>().ToListAsync();
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
    }
}
