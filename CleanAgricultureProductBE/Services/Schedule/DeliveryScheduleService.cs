using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories.Order;
using CleanAgricultureProductBE.Repositories.Schedule;

namespace CleanAgricultureProductBE.Services.Schedule
{
    public class DeliveryScheduleService : IDeliveryScheduleService
    {
        private readonly IDeliveryScheduleRepository _scheduleRepo;
        private readonly IOrderRepository _orderRepo;

        public DeliveryScheduleService(
            IDeliveryScheduleRepository scheduleRepo,
            IOrderRepository orderRepo)
        {
            _scheduleRepo = scheduleRepo;
            _orderRepo = orderRepo;
        }

        public async Task<IEnumerable<DeliverySchedule>> GetAllAsync()
        {
            return await _scheduleRepo.GetAllAsync();
        }

        public async Task<DeliverySchedule?> GetByIdAsync(Guid id)
        {
            return await _scheduleRepo.GetByIdAsync(id);
        }

        public async Task CreateAsync(Guid orderId, DateTime deliveryTime)
        {
            var order = await _orderRepo.GetOrderById(orderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.OrderStatus != "Confirmed")
                throw new Exception("Order must be confirmed before scheduling");

            var schedule = new DeliverySchedule
            {
                DeliveryScheduleId = Guid.NewGuid(),
                OrderId = orderId,
                ScheduledDate = deliveryTime,
                Status = DeliveryStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _scheduleRepo.AddAsync(schedule);

            order.OrderStatus = "Scheduled";
            await _orderRepo.UpdateAsync(order);

            await _scheduleRepo.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(Guid scheduleId, string status)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(scheduleId);

            if (schedule == null)
                throw new Exception("Schedule not found");

            if (!Enum.TryParse<DeliveryStatus>(status, true, out var parsedStatus))
                throw new Exception("Invalid status value");

            schedule.Status = parsedStatus;

            await _scheduleRepo.UpdateAsync(schedule);
            await _scheduleRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var schedule = await _scheduleRepo.GetByIdAsync(id);

            if (schedule == null)
                throw new Exception("Schedule not found");

            await _scheduleRepo.DeleteAsync(schedule);
            await _scheduleRepo.SaveChangesAsync();
        }
    }
}
