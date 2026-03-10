using CleanAgricultureProductBE.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.CycleSchedule
{
    public class CycleScheduleRepository(AppDbContext context) : ICycleScheduleRepository
    {
        public async Task<List<Models.CycleSchedule>> GetCycleSchedules()
        {
           return await context.CycleSchedules
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Models.CycleSchedule> GetCycleScheduleByOrderId(Guid orderId)
        {
            return await context.CycleSchedules.FirstOrDefaultAsync(c => c.OrderId == orderId);
        }

        public async Task AddCycleSchedule(Models.CycleSchedule cycleSchedule)
        {
            context.CycleSchedules.Add(cycleSchedule);
            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckOrderIsCycleSchedule(Guid orderId)
        {
            return await context.CycleSchedules.AnyAsync(cs => cs.OrderId == orderId);
        }

        public async Task UpdateCycleSchedule(Models.CycleSchedule cycleSchedule)
        {
            context.CycleSchedules.Update(cycleSchedule);
            await context.SaveChangesAsync();
        }
    }
}
