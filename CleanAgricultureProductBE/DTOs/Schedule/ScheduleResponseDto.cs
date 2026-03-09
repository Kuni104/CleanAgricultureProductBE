namespace CleanAgricultureProductBE.DTOs.Schedule
{
    public class ScheduleResponseDto
    {
        public Guid ScheduleId { get; set; }
        public Guid DeliveryPersonId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
