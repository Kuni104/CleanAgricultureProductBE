using CleanAgricultureProductBE.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.DeliveryFee
{
    public class DeliveryFeeRepository(AppDbContext context) : IDeliveryFeeRepository
    {
        public async Task<List<Models.DeliveryFee>> GetDeliveryFeeList()
        {
            return await context.DeliveryFees.ToListAsync();
        }

        public async Task AddDeliveryFee(Models.DeliveryFee newDeliveryFee)
        {
            context.DeliveryFees.Add(newDeliveryFee);
            await context.SaveChangesAsync();
        }
    }
}
