using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories.DSchedule
{
    public interface IScheduleRepository
    {
        Task AddAsync(Models.Schedule schedule);
        Task<Models.Schedule?> GetByIdAsync(Guid scheduleId);
        Task<List<Models.Order>> GetOrdersByIdsAsync(List<Guid> orderIds);
        Task SaveChangesAsync();
        Task<Schedule?> GetByDeliveryPersonAndDateAsync(Guid deliveryPersonId, DateTime date);
    }
}
