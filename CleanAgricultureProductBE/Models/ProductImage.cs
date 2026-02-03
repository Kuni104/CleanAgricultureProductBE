namespace CleanAgricultureProductBE.Models
{
    public class ProductImage
    {
        public Guid ProductImageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public Guid ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}
