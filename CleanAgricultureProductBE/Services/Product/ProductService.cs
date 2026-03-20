using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Enum;
using CleanAgricultureProductBE.Services.Image;
using CleanAgricultureProductBE.Repositories.Product;
using ProductModel = CleanAgricultureProductBE.Models.Product;

namespace CleanAgricultureProductBE.Services.Product
{
    public static class ProductStatus
    {
        public const string Active = "Active";
        public const string Inactive = "Inactive";
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IProductImageService _productImageService;

        public ProductService(IProductRepository productRepo, IProductImageService productImageService)
        {
            _productRepo = productRepo;
            _productImageService = productImageService;
        }

        public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Product name is required");

            if (dto.Name.Trim().Length > 200)
                throw new Exception("Product name must not exceed 200 characters");

            if (dto.Price <= 0)
                throw new Exception("Price must be greater than 0");

            if (string.IsNullOrWhiteSpace(dto.Unit))
                throw new Exception("Unit is required");

            if (dto.Stock < 0)
                throw new Exception("Stock must be greater than or equal to 0");

            if (dto.ExpiredAt <= dto.ImportedAt)
                throw new Exception("ExpiredAt phải lớn hơn ImportedAt");

            var product = new ProductModel
            {
                ProductId = Guid.NewGuid(),
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Unit = dto.Unit,
                Stock = dto.Stock,
                Status = ProductStatus.Active,
                ImportedAt = dto.ImportedAt,
                ExpiredAt = dto.ExpiredAt
            };

            var created = await _productRepo.CreateAsync(product);
            var result = await _productRepo.GetByIdAsync(created.ProductId);

            if (dto.Images != null && dto.Images.Count > 0)
            {
                await _productImageService.UploadProductImagesAsync(result!.ProductId, dto.Images);
                result = await _productRepo.GetByIdAsync(created.ProductId);
            }

            return new ProductResponseDto
            {
                ProductId = result!.ProductId,
                CategoryId = result.CategoryId,
                CategoryName = result.Category.Name,
                Name = result.Name,
                Description = result.Description,
                Price = result.Price,
                Unit = result.Unit,
                Stock = result.Stock,
                Status = result.Status,
                ImportedAt = result.ImportedAt,
                ExpiredAt = result.ExpiredAt,
                ImageUrls = result.ProductImages.Select(pi => pi.ImageUrl).ToList()
            };
        }

        public async Task<List<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _productRepo.GetAllAsync();
            return products
                .Where(p => p.Status == ProductStatus.Active)
                .Select(p => new ProductResponseDto
                {
                    ProductId = p.ProductId,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Unit = p.Unit,
                    Stock = p.Stock,
                    Status = p.Status,
                    ImportedAt = p.ImportedAt,
                    ExpiredAt = p.ExpiredAt,
                    ImageUrls = p.ProductImages.Select(pi => pi.ImageUrl).ToList()
                }).ToList();
        }

        public async Task<ProductResponseDto> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null || product.IsDeleted)
                throw new Exception("Product not found");

            return new ProductResponseDto
            {
                ProductId = product.ProductId,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Unit = product.Unit,
                Stock = product.Stock,
                Status = product.Status,
                ImportedAt = product.ImportedAt,
                ExpiredAt = product.ExpiredAt,
                ImageUrls = product.ProductImages.Select(pi => pi.ImageUrl).ToList()
            };
        }

        public async Task<ProductResponseDto> UpdateProductAsync(Guid id, UpdateProductDto dto)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null || product.IsDeleted)
                throw new Exception("Product not found");

            if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name.Trim().Length > 200)
                throw new Exception("Product name must not exceed 200 characters");

            if (dto.Price.HasValue && dto.Price.Value <= 0)
                throw new Exception("Price must be greater than 0");

            if (dto.Stock.HasValue && dto.Stock.Value < 0)
                throw new Exception("Stock must be greater than or equal to 0");

            var effectiveImportedAt = dto.ImportedAt ?? product.ImportedAt;
            var effectiveExpiredAt = dto.ExpiredAt ?? product.ExpiredAt;

            if (effectiveExpiredAt <= effectiveImportedAt)
                throw new Exception("ExpiredAt phải lớn hơn ImportedAt");

            if (dto.CategoryId.HasValue)
                product.CategoryId = dto.CategoryId.Value;
            if (!string.IsNullOrWhiteSpace(dto.Name))
                product.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Description))
                product.Description = dto.Description;
            if (dto.Price.HasValue)
                product.Price = dto.Price.Value;
            if (!string.IsNullOrWhiteSpace(dto.Unit))
                product.Unit = dto.Unit;
            if (dto.Stock.HasValue)
                product.Stock = dto.Stock.Value;
            if (dto.ImportedAt.HasValue)
                product.ImportedAt = dto.ImportedAt.Value;
            if (dto.ExpiredAt.HasValue)
                product.ExpiredAt = dto.ExpiredAt.Value;

            var updated = await _productRepo.UpdateAsync(product);
            var result = await _productRepo.GetByIdAsync(updated.ProductId);

            if (dto.Images != null && dto.Images.Count > 0)
            {
                await _productImageService.UploadProductImagesAsync(result!.ProductId, dto.Images);
                result = await _productRepo.GetByIdAsync(updated.ProductId);
            }

            return new ProductResponseDto
            {
                ProductId = result!.ProductId,
                CategoryId = result.CategoryId,
                CategoryName = result.Category.Name,
                Name = result.Name,
                Description = result.Description,
                Price = result.Price,
                Unit = result.Unit,
                Stock = result.Stock,
                Status = result.Status,
                ImportedAt = result.ImportedAt,
                ExpiredAt = result.ExpiredAt,
                ImageUrls = result.ProductImages.Select(pi => pi.ImageUrl).ToList()
            };
        }

        public async Task<bool> DeleteProductAsync(Guid id, bool confirm)
        {
            if (!confirm)
                throw new Exception("Delete confirmation required");

            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
                throw new Exception("Product not found");

            return await _productRepo.DeleteAsync(id);
        }

        public async Task<bool> UpdateProductStatusAsync(Guid productId, string status)
        {
            var product = await _productRepo.GetByIdAsync(productId);

            if (product == null || product.IsDeleted)
            {
                return false;
            }

            product.Status = status;
            await _productRepo.UpdateAsync(product);

            return true;
        }

        public async Task<ResponseDtoWithPagination<List<ProductResponseDto>>> GetAllProductsWithPaginationAsync(
            int? page, int? size, Guid? categoryId, string? keyword, decimal? minPrice, decimal? maxPrice, ProductStatusEnum productStatus)
        {
            int pageSize = size ?? 10;
            int pageNumber = page ?? 1;

            if (pageNumber <= 0)
                throw new ArgumentException("Số trang (page) phải lớn hơn 0");
            if (pageSize <= 0)
                throw new ArgumentException("Kích thước trang (size) phải lớn hơn 0");

            int offset = (pageNumber - 1) * pageSize;

            var products = await _productRepo.GetAllWithPaginationAsync(offset, pageSize, categoryId, keyword, minPrice, maxPrice, productStatus);
            var total = await _productRepo.CountAllAsync(categoryId, keyword, minPrice, maxPrice, productStatus);

            var result = new ResponseDtoWithPagination<List<ProductResponseDto>>
            {
                ResultObject = products
                    //.Where(p => p.Status == ProductStatus.Active)
                    .Select(p => new ProductResponseDto
                    {
                        ProductId = p.ProductId,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category?.Name,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        Unit = p.Unit,
                        Stock = p.Stock,
                        Status = p.Status,
                        ImportedAt = p.ImportedAt,
                        ExpiredAt = p.ExpiredAt,
                        ImageUrls = p.ProductImages.Select(pi => pi.ImageUrl).ToList()
                    }).ToList(),
                Pagination = new Pagination
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalItems = total,
                    TotalPages = (int)Math.Ceiling((double)total / pageSize)
                }
            };

            return result;
        }
    }
}
