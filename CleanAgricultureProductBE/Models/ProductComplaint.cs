namespace CleanAgricultureProductBE.Models
{
    public class ProductComplaint
    {
        public Guid ProductComplaintId { get; set; }
        public Guid ComplaintId { get; set; }
        public Guid ProductId { get; set; }
        /*------------------------------------------------------------------------------------------------------------------------*/
        public Complaint Complaint { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
