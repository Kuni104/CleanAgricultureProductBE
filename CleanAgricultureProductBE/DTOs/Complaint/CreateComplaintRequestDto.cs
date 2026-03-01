namespace CleanAgricultureProductBE.DTOs.Complaint
{
    public class CreateComplaintRequestDto
    {
        public Guid OrderId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Evidence { get; set; }
        // Optional: list of product IDs to attach as product complaints
        public List<Guid>? ProductIds { get; set; }
    }
}
