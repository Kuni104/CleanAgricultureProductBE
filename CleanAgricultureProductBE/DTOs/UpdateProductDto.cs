using Microsoft.AspNetCore.Http;

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
        public DateTime? ImportedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}