namespace CleanAgricultureProductBE.Models
{
    public class Complaint
    {
        public Guid ComplaintId { get; set; }
        public Guid? StaffId { get; set; }
        public Guid OrderId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending | Resolved | Rejected
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolveAt { get; set; }
        public string Evidence { get; set; } = string.Empty;
        public string? Resolution { get; set; } // Exchange | Refund (set when Resolved)
        /*------------------------------------------------------------------------------------------------------------------------*/
        public Account? Staff { get; set; }
        public Order Order { get; set; } = null!;
        public ICollection<ProductComplaint> ProductComplaints { get; set; } = new List<ProductComplaint>();
    }
}
