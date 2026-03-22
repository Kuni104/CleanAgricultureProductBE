using CleanAgricultureProductBE.DTOs.CycleSchedule;

namespace CleanAgricultureProductBE.Repositories.CycleSchedule
{
    public interface ICycleScheduleRepository
    {
        public Task<List<Models.CycleSchedule>> GetCycleSchedules();
        public Task<List<Models.CycleSchedule>> GetCycleSchedulesByUser(Guid accountId);
        public Task<Models.CycleSchedule?> GetCycleScheduleByOrderId(Guid orderId);
        public Task<Models.CycleSchedule?> GetCycleScheduleById(Guid id);
        public Task AddCycleSchedule(Models.CycleSchedule cycleSchedule);
        public Task<bool> CheckOrderIsCycleSchedule(Guid orderId);
        public Task UpdateCycleSchedule(Models.CycleSchedule cycleSchedule);
    }
}
