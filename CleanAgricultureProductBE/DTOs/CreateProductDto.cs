using Microsoft.AspNetCore.Http;

namespace CleanAgricultureProductBE.DTOs
{
    public class CreateProductDto
    {
        public Guid CategoryId{get;set;}
        public string Name{get;set;} = string.Empty;
        public string Description{get;set;} = string.Empty;
        public decimal Price{get;set;}
        public string Unit{get;set;} = string.Empty;
        public int Stock{get;set;}
        public DateTime ImportedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
        public List<IFormFile> Images { get; set; } = new();

    }

}