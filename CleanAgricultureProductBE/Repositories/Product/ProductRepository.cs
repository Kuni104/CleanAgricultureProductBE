using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.Enum;
using Microsoft.EntityFrameworkCore;
using ProductModel = CleanAgricultureProductBE.Models.Product;

namespace CleanAgricultureProductBE.Repositories.Product
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductModel> CreateAsync(ProductModel product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<List<ProductModel>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public Task<ProductModel?> GetByIdAsync(Guid id)
        {
            return _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<ProductModel> UpdateAsync(ProductModel product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await GetByIdAsync(id);
            if (product == null) return false;

            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ProductModel>> GetAllWithPaginationAsync(int offset, int pageSize, Guid? categoryId, string? keyword, decimal? minPrice, decimal? maxPrice, ProductStatusEnum productStatus)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (productStatus == ProductStatusEnum.Inactive)
                query = query.Where(p => p.Status == "Inactive");

            if (productStatus == ProductStatusEnum.Active)
                query = query.Where(p => p.Status == "Active");

            // Filter by category
            if (categoryId.HasValue && categoryId != Guid.Empty)
                query = query.Where(p => p.CategoryId == categoryId);

            // Search by keyword (product name)
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.Name.Contains(keyword));

            // Filter by price range
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            return await query
                    .OrderByDescending(p => p.ProductId)
                .Skip(offset)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAllAsync(Guid? categoryId, string? keyword, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.Products.AsQueryable();

            // Filter by category
            if (categoryId.HasValue && categoryId != Guid.Empty)
                query = query.Where(p => p.CategoryId == categoryId);

            // Search by keyword (product name)
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.Name.Contains(keyword));

            // Filter by price range
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);
            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            return await query.CountAsync();
        }
    }
}
