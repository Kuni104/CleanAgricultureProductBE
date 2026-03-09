namespace CleanAgricultureProductBE.DTOs.ApiResponse
{
    public class CreateScheduleRequestDto
    {
        public Guid DeliveryPersonId { get; set; }
        public DateTime ScheduledDate { get; set; }
    }
}
