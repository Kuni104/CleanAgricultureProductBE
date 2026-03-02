namespace CleanAgricultureProductBE.Services.Schedule
{
    public interface IScheduleService
    {
        Task<Guid> CreateScheduleAsync(Guid deliveryPersonId, DateTime scheduledDate);
        Task AssignOrdersAsync(Guid scheduleId, List<Guid> orderIds);
    }
}
