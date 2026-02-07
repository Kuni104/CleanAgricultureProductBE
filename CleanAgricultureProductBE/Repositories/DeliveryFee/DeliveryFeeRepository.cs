using CleanAgricultureProductBE.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.DeliveryFee
{
    public class DeliveryFeeRepository(AppDbContext context) : IDeliveryFeeRepository
    {
        public async Task<Models.DeliveryFee?> GetDeliveryFeeById(Guid deliveryFeeId)
        {
            return await context.DeliveryFees.FirstOrDefaultAsync(df => df.DeliveryFeeId == deliveryFeeId);
        }
        public async Task<List<Models.DeliveryFee>> GetDeliveryFeeList()
        {
            return await context.DeliveryFees.ToListAsync();
        }

        public async Task AddDeliveryFee(Models.DeliveryFee newDeliveryFee)
        {
            context.DeliveryFees.Add(newDeliveryFee);
            await context.SaveChangesAsync();
        }

        public async Task UpdateDeliveryFee(Models.DeliveryFee updatedDeliveryFee)
        {
            context.DeliveryFees.Update(updatedDeliveryFee);
            await context.SaveChangesAsync();
        }

        public async Task DeleteDeliveryFee(Models.DeliveryFee deliveryFeeToRemove)
        {
            context.DeliveryFees.Remove(deliveryFeeToRemove);
            await context.SaveChangesAsync();
        }
    }
}
