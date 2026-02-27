using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.Schedule
{
    public class DeliveryScheduleRepository : IDeliveryScheduleRepository
    {
        private readonly AppDbContext _context;

        public DeliveryScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DeliverySchedule>> GetAllAsync()
        {
            return await _context.DeliverySchedules
                .Include(x => x.Order)
                .ToListAsync();
        }

        public async Task<DeliverySchedule?> GetByIdAsync(Guid id)
        {
            return await _context.DeliverySchedules
                .Include(x => x.Order)
                .FirstOrDefaultAsync(x => x.DeliveryScheduleId == id);
        }

        public async Task AddAsync(DeliverySchedule schedule)
        {
            await _context.DeliverySchedules.AddAsync(schedule);
        }

        public async Task UpdateAsync(DeliverySchedule schedule)
        {
            _context.DeliverySchedules.Update(schedule);
        }

        public async Task DeleteAsync(DeliverySchedule schedule)
        {
            _context.DeliverySchedules.Remove(schedule);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
