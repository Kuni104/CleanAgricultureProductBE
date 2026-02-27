using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Services.Schedule
{
    public interface IDeliveryScheduleService
    {
        Task<IEnumerable<DeliverySchedule>> GetAllAsync();
        Task<DeliverySchedule?> GetByIdAsync(Guid id);
        Task CreateAsync(Guid orderId, DateTime deliveryTime);
        Task UpdateStatusAsync(Guid scheduleId, string status);
        Task DeleteAsync(Guid id);
    }
}
