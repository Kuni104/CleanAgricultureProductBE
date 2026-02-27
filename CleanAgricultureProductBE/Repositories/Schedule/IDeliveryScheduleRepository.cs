using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories.Schedule
{
    public interface IDeliveryScheduleRepository
    {
        Task<IEnumerable<DeliverySchedule>> GetAllAsync();
        Task<DeliverySchedule?> GetByIdAsync(Guid id);
        Task AddAsync(DeliverySchedule schedule);
        Task UpdateAsync(DeliverySchedule schedule);
        Task DeleteAsync(DeliverySchedule schedule);
        Task SaveChangesAsync();
    }
}
