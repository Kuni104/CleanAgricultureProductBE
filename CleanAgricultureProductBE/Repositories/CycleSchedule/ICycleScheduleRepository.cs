using CleanAgricultureProductBE.DTOs.CycleSchedule;

namespace CleanAgricultureProductBE.Repositories.CycleSchedule
{
    public interface ICycleScheduleRepository
    {
        public Task<List<Models.CycleSchedule>> GetCycleSchedules();
        public Task<Models.CycleSchedule> GetCycleScheduleByOrderId(Guid orderId);
        public Task AddCycleSchedule(Models.CycleSchedule cycleSchedule);
        public Task<bool> CheckOrderIsCycleSchedule(Guid orderId);
        public Task UpdateCycleSchedule(Models.CycleSchedule cycleSchedule);
    }
}
