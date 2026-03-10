namespace CleanAgricultureProductBE.DTOs.Complaint
{
    public class ComplaintResponseDto
    {
        public Guid ComplaintId { get; set; }
        public Guid OrderId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Evidence { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolveAt { get; set; }
        public string? StaffName { get; set; }
        public List<ProductComplaintResponseDto> ProductComplaints { get; set; } = [];
        public List<ComplaintImageResponseDto> Images { get; set; } = new();
    }

    public class ProductComplaintResponseDto
    {
        public Guid ProductComplaintId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
    }
}
