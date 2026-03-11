using CleanAgricultureProductBE.Data;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.DeliveryFee
{
    public class DeliveryFeeRepository(AppDbContext context) : IDeliveryFeeRepository
    {
        public async Task<Models.DeliveryFee?> GetDeliveryFeeById(Guid deliveryFeeId)
        {
            return await context.DeliveryFees.FirstOrDefaultAsync(df => df.DeliveryFeeId == deliveryFeeId);
        }

        public async Task<bool> CheckDeliveryFee(string city, string ward, string district)
        {
            return await context.DeliveryFees.AnyAsync(df => df.City == city && df.Ward == ward && df.District == district);
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

        public async Task<Models.DeliveryFee?> GetDeliveryFeeByWard(string ward)
        {
            return await context.DeliveryFees
                                .Where(df => df.Ward.ToLower() == ward.Trim().ToLower())
                                .OrderByDescending(df => df.FeeAmount)
                                .ThenBy(df => df.DeliveryFeeId)
                                .FirstOrDefaultAsync();
        }

        public async Task<Models.DeliveryFee?> GetHighestDeliveryFee()
        {
            return await context.DeliveryFees
                        .OrderByDescending(df => df.FeeAmount)
                        .FirstOrDefaultAsync();
        }
    }
}
