namespace CleanAgricultureProductBE.Models
{
    public class ProductConplaintImage
    {
        public Guid Id { get; set; }

        public Guid ProductComplaintId { get; set; }

        public string ImageUrl { get; set; }

        public ProductComplaint ProductComplaint { get; set; }
    }
}
