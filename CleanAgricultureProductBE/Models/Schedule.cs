namespace CleanAgricultureProductBE.Models
{
    public class Schedule
    {
        public Guid ScheduleId { get; set; }
        public Guid DeliveryPersonId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        /*------------------------------------------------------------------------------------------------------------------------*/
        public Account DeliveryPerson { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
