namespace CleanAgricultureProductBE.Models
{
    public class ProductImage
    {
        public Guid ProductImageId { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        /*------------------------------------------------------------------------------------------------------------------------*/
        public Product Product { get; set; } = null!;
    }
}
