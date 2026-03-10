using CleanAgricultureProductBE.Data;
using Microsoft.EntityFrameworkCore;
using CategoryModel = CleanAgricultureProductBE.Models.Category;

namespace CleanAgricultureProductBE.Repositories.Category
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryModel> CreateAsync(CategoryModel category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<List<CategoryModel>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<CategoryModel?> GetByIdAsync(Guid id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<CategoryModel?> GetByNameAsync(string name)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<CategoryModel> UpdateAsync(CategoryModel category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await GetByIdAsync(id);
            if (category == null) return false;

            category.IsDeleted = true;
            category.DeletedAt = DateTime.UtcNow;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
