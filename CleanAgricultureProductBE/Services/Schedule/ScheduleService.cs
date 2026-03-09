using CleanAgricultureProductBE.DTOs.ApiResponse;
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

    private ScheduleResponseDto MapToDto(Schedule schedule)
    {
        return new ScheduleResponseDto
        {
            ScheduleId = schedule.ScheduleId,
            DeliveryPersonId = schedule.DeliveryPersonId,
            ScheduledDate = schedule.ScheduledDate,
            Status = schedule.Status,
            TotalOrders = schedule.Orders.Count,
            CreatedAt = schedule.CreatedAt,
            UpdatedAt = schedule.UpdatedAt
        };
    }

    public async Task<Guid> CreateScheduleAsync(Guid deliveryPersonId, DateTime scheduledDate)
    {
        scheduledDate = scheduledDate.Date;

        var existingSchedule = await _repository
            .GetByDeliveryPersonAndDateAsync(deliveryPersonId, scheduledDate);

        if (existingSchedule != null)
            throw new Exception("Delivery person already has a schedule on this date");

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
            if (order.ScheduleId != null)
                throw new Exception($"Order {order.OrderId} already assigned to a schedule");

            order.ScheduleId = schedule.ScheduleId;
        }

        schedule.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
    }
}