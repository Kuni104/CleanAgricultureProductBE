using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> CreateAsync(Category category);
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category> UpdateAsync(Category category);
        Task<bool> DeleteAsync(Guid id);
    }
}
