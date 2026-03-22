using CleanAgricultureProductBE.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.CycleSchedule
{
    public class CycleScheduleRepository(AppDbContext context) : ICycleScheduleRepository
    {
        public async Task<List<Models.CycleSchedule>> GetCycleSchedules()
        {
           return await context.CycleSchedules
                .Include(cs => cs.Order)
                .ThenInclude(cso => cso.Schedule)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Models.CycleSchedule>> GetCycleSchedulesByUser(Guid accountId)
        {
            return await context.CycleSchedules
            .Include(cs => cs.Order)
            .ThenInclude(o => o.Customer)
            .Include(cs => cs.Order)
            .ThenInclude(o => o.Schedule)
            .Where(cs => cs.Order.Customer.AccountId == accountId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        }

        public async Task<Models.CycleSchedule?> GetCycleScheduleByOrderId(Guid orderId)
        {
            return await context.CycleSchedules
                .Include(cs => cs.Order)
                .ThenInclude(cso => cso.Schedule)
                .FirstOrDefaultAsync(c => c.OrderId == orderId);
        }

        public async Task AddCycleSchedule(Models.CycleSchedule cycleSchedule)
        {
            context.CycleSchedules.Add(cycleSchedule);
            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckOrderIsCycleSchedule(Guid orderId)
        {
            return await context.CycleSchedules.Where(cs => cs.Status == "Active").AnyAsync(cs => cs.OrderId == orderId);
        }

        public async Task UpdateCycleSchedule(Models.CycleSchedule cycleSchedule)
        {
            context.CycleSchedules.Update(cycleSchedule);
            await context.SaveChangesAsync();
        }

        public async Task<Models.CycleSchedule?> GetCycleScheduleById(Guid id)
        {
            return await context.CycleSchedules.Include(cs => cs.Order).ThenInclude(cso => cso.Schedule).Where(cs => cs.CycleScheduleId == id).FirstOrDefaultAsync();
        }
    }
}
