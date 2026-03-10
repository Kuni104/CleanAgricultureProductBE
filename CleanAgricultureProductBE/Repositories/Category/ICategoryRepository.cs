using CategoryModel = CleanAgricultureProductBE.Models.Category;

namespace CleanAgricultureProductBE.Repositories.Category
{
    public interface ICategoryRepository
    {
        Task<CategoryModel> CreateAsync(CategoryModel category);
        Task<List<CategoryModel>> GetAllAsync();
        Task<CategoryModel?> GetByIdAsync(Guid id);
        Task<CategoryModel?> GetByNameAsync(string name);
        Task<CategoryModel> UpdateAsync(CategoryModel category);
        Task<bool> DeleteAsync(Guid id);
    }
}
