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
            if (dto.Price <= 0)
                throw new Exception("Price must be greater than 0");

            var unit = int.Parse(dto.Unit);
            if (unit <= 0)
                throw new Exception("Unit must be greater than 0");

            if (dto.Stock < 0)
                throw new Exception("Stock must be greater than or equal to 0");

            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Unit = unit,
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

        public async Task<List<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _productRepo.GetAllAsync();
            return products
                .Where(p => p.Status == "Active")
                .Select(p => new ProductResponseDto
                {
                    ProductId = p.ProductId,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Unit = p.Unit.ToString(),
                    Stock = p.Stock,
                    Status = p.Status
                }).ToList();
        }

        public async Task<ProductResponseDto> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null || product.Status == "Inactive")
                throw new Exception("Product not found");

            return new ProductResponseDto
            {
                ProductId = product.ProductId,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Unit = product.Unit.ToString(),
                Stock = product.Stock,
                Status = product.Status
            };
        }

        public async Task<ProductResponseDto> UpdateProductAsync(Guid id, UpdateProductDto dto)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null || product.Status == "Inactive")
                throw new Exception("Product not found");

            if (dto.Price.HasValue && dto.Price.Value <= 0)
                throw new Exception("Price must be greater than 0");

            if (!string.IsNullOrWhiteSpace(dto.Unit))
            {
                var unit = int.Parse(dto.Unit);
                if (unit <= 0)
                    throw new Exception("Unit must be greater than 0");
                product.Unit = unit;
            }

            if (dto.Stock.HasValue && dto.Stock.Value < 0)
                throw new Exception("Stock must be greater than or equal to 0");

            if (dto.CategoryId.HasValue)
                product.CategoryId = dto.CategoryId.Value;
            if (!string.IsNullOrWhiteSpace(dto.Name))
                product.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Description))
                product.Description = dto.Description;
            if (dto.Price.HasValue)
                product.Price = dto.Price.Value;
            if (dto.Stock.HasValue)
                product.Stock = dto.Stock.Value;

            var updated = await _productRepo.UpdateAsync(product);
            var result = await _productRepo.GetByIdAsync(updated.ProductId);

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

        public async Task<bool> DeleteProductAsync(Guid id, bool confirm)
        {
            if (!confirm)
                throw new Exception("Delete confirmation required");

            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
                throw new Exception("Product not found");

            if (product.Status == "Inactive")
                throw new Exception("Product already deleted");

            product.Status = "Inactive";
            await _productRepo.UpdateAsync(product);
            return true;
        }

        public async Task<bool> UpdateProductStatusAsync(Guid productId, string status)
        {
            var product = await _productRepo.GetByIdAsync(productId);

            if (product == null)
            {
                return false;
            }

            product.Status = status;
            await _productRepo.UpdateAsync(product);

            return true;
        }
    }
}
