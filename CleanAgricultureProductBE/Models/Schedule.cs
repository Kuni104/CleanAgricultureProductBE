namespace CleanAgricultureProductBE.Models
{
    public class Schedule
    {
        public Guid Id { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string status { get; set; } = string.Empty;
        public Guid DeliveryPersonId { get; set; }

        public Account DeliveryPerson { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
