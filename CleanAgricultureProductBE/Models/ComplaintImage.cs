namespace CleanAgricultureProductBE.Models
{
    public class ComplaintImage
    {
        public Guid Id { get; set; }
        public Guid ComplaintId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public Complaint Complaint { get; set; } = null!;
    }
}
