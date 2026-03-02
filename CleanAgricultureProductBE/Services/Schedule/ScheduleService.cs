using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories.DSchedule;
using CleanAgricultureProductBE.Services.Schedule;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _repository;

    public ScheduleService(IScheduleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> CreateScheduleAsync(Guid deliveryPersonId, DateTime scheduledDate)
    {
        var schedule = new Schedule
        {
            ScheduleId = Guid.NewGuid(),
            DeliveryPersonId = deliveryPersonId,
            ScheduledDate = scheduledDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = "Pending"
        };

        await _repository.AddAsync(schedule);
        await _repository.SaveChangesAsync();

        return schedule.ScheduleId;
    }

    public async Task AssignOrdersAsync(Guid scheduleId, List<Guid> orderIds)
    {
        var schedule = await _repository.GetByIdAsync(scheduleId);

        if (schedule == null)
            throw new Exception("Schedule not found");

        var orders = await _repository.GetOrdersByIdsAsync(orderIds);

        if (!orders.Any())
            throw new Exception("No valid orders found");

        foreach (var order in orders)
        {
            // nếu order đã có schedule rồi thì bỏ qua
            if (order.ScheduleId != null)
                continue;

            order.ScheduleId = schedule.ScheduleId;
        }

        schedule.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
    }
}