using CleanAgricultureProductBE.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.CycleSchedule
{
    public class CycleScheduleRepository(AppDbContext context) : ICycleScheduleRepository
    {
        public async Task<List<Models.CycleSchedule>> GetCycleSchedules()
        {
           return await context.CycleSchedules.ToListAsync();
        }
    }
}
