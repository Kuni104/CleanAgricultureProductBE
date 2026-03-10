using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories.Image
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext _context;
        public ProductImageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddImagesAsync(List<ProductImage> images)
        {
            await _context.ProductImages.AddRangeAsync(images);
            await _context.SaveChangesAsync();
        }
    }
}
