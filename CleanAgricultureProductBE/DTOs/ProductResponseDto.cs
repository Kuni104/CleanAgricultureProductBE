namespace CleanAgricultureProductBE.DTOs
{
    public class ProductResponseDto
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int Stock { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}