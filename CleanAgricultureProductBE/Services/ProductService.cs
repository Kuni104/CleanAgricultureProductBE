using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories;

namespace CleanAgricultureProductBE.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
        {
            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Unit = int.Parse(dto.Unit),
                Stock = dto.Stock,
                Status = "Active"
            };

            var created = await _productRepo.CreateAsync(product);
            var result = await _productRepo.GetByIdAsync(created.ProductId);

            return new ProductResponseDto
            {
                ProductId = result!.ProductId,
                CategoryId = result.CategoryId,
                CategoryName = result.Category.Name,
                Name = result.Name,
                Description = result.Description,
                Price = result.Price,
                Unit = result.Unit.ToString(),
                Stock = result.Stock,
                Status = result.Status
            };
        }
    }
}
