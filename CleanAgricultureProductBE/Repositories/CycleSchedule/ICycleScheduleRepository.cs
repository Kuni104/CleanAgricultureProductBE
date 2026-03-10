using CleanAgricultureProductBE.DTOs.CycleSchedule;

namespace CleanAgricultureProductBE.Repositories.CycleSchedule
{
    public interface ICycleScheduleRepository
    {
        public Task<List<Models.CycleSchedule>> GetCycleSchedules();
    }
}
