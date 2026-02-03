namespace CleanAgricultureProductBE.Models
{
    public class Complaint
    {
        public Guid ComplaintId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ResolveAt { get; set; }
        public string Evidence { get; set; } = string.Empty;
        public Guid StaffId { get; set; }
        public Guid OrderId { get; set; }

        public Account Staff { get; set; } = null!;
        public Order Order { get; set; } = null!;

    }
}
