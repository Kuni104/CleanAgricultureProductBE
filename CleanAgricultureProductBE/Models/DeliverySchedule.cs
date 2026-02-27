namespace CleanAgricultureProductBE.Models
{
    public class DeliverySchedule
    {
        public Guid DeliveryScheduleId { get; set; }

        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public DateTime ScheduledDate { get; set; }

        public DeliveryStatus Status { get; set; } // Pending, Shipping, Delivered, Failed

        public Guid? AssignedStaffId { get; set; }
        public Account? AssignedStaff { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
