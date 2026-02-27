namespace CleanAgricultureProductBE.DTOs.Schedule
{
    public class CreateScheduleDto
    {
        public Guid OrderId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public Guid StaffId { get; set; }
    }
}
