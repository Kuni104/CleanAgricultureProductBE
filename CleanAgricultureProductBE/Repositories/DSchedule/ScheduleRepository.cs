using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories.DSchedule;
using Microsoft.EntityFrameworkCore;

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
}