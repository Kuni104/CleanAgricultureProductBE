namespace CleanAgricultureProductBE.DTOs
{
    public class UpdateProductDto
    {
        public Guid? CategoryId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Unit { get; set; }
        public int? Stock { get; set; }
    }
}