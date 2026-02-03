namespace CleanAgricultureProductBE.Models
{
    public class CycleSchedule
    {
        public Guid CycleScheduleId { get; set; }
        public Guid OrderId { get; set; }
        public int DayCycle { get; set; }
        public bool isMonthly { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        /*------------------------------------------------------------------------------------------------------------------------*/
        public Order Order { get; set; } = null!;
    }
}
