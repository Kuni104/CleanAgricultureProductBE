using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories.DSchedule;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class ScheduleRepository : IScheduleRepository
{
    private readonly AppDbContext _context;

    public ScheduleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Schedule schedule)
    {
        await _context.Schedules.AddAsync(schedule);
    }

    public async Task<Schedule?> GetByIdAsync(Guid scheduleId)
    {
        return await _context.Schedules
            .Include(s => s.Orders)
            .FirstOrDefaultAsync(s => s.ScheduleId == scheduleId);
    }

    public async Task<List<Order>> GetOrdersByIdsAsync(List<Guid> orderIds)
    {
        return await _context.Orders
            .Where(o => orderIds.Contains(o.OrderId))
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Schedule?> GetByDeliveryPersonAndDateAsync(Guid deliveryPersonId, DateTime date)
    {
        return await _context.Schedules
            .Include(s => s.Orders)
            .FirstOrDefaultAsync(x =>
                x.DeliveryPersonId == deliveryPersonId &&
                x.ScheduledDate.Date == date.Date);
    }

    public async Task<List<Schedule>> GetByDeliveryPerson(Guid deliveryPersonId)
    {
        return await _context.Schedules
            .Where(s => s.DeliveryPersonId == deliveryPersonId)
            .ToListAsync();
    }

    public async Task<List<Schedule>> GetByDeliveryPersonWithPagination(Guid deliveryPersonId, int offset, int pageSize)
    {
        return await _context.Schedules
            .OrderByDescending(s => s.ScheduledDate)
            .Where(s => s.DeliveryPersonId == deliveryPersonId)
            .Skip(offset)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Schedule>> GetSchedulesToday()
    {
        return await _context.Schedules
            .Include(s => s.Orders)
            .Where(x =>
                x.ScheduledDate.Date == DateTime.UtcNow.Date)
            .OrderByDescending(s => s.ScheduledDate)
            .ToListAsync();
    }
}