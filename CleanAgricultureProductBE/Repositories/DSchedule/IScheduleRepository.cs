using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories.DSchedule
{
    public interface IScheduleRepository
    {
        Task AddAsync(Models.Schedule schedule);
        Task<Models.Schedule?> GetByIdAsync(Guid scheduleId);
        Task<List<Models.Order>> GetOrdersByIdsAsync(List<Guid> orderIds);
        Task SaveChangesAsync();
        public Task UpdateAsync(Schedule schedule);
        public Task<Schedule?> GetByDeliveryPersonAndDateAsync(Guid deliveryPersonId, DateTime date);
        public Task<List<Schedule>> GetByDeliveryPerson(Guid deliveryPersonId);
        public Task<List<Schedule>> GetSchedulesToday();
        public Task<List<Schedule>> GetSchedulesByDate(DateTime date);
        public Task<List<Schedule>> GetByDeliveryPersonWithPagination(Guid deliveryPersonId, int offset, int pageSize);
    }
}
