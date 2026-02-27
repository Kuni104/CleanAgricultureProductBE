namespace CleanAgricultureProductBE.DTOs.Schedule
{
    public class UpdateScheduleStatusDto
    {
        public Guid ScheduleId { get; set; }
        public string Status { get; set; }
    }
}
