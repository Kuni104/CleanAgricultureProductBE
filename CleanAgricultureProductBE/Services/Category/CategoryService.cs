using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.Repositories.Category;
using CategoryModel = CleanAgricultureProductBE.Models.Category;

namespace CleanAgricultureProductBE.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryService(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new Exception("Category name is required");

            if (dto.Name.Trim().Length > 100)
                throw new Exception("Category name must not exceed 100 characters");

            var existing = await _categoryRepo.GetByNameAsync(dto.Name.Trim());
            if (existing != null)
                throw new Exception("Category name already exists");

            var category = new CategoryModel
            {
                CategoryId = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Description = dto.Description,
                Status = "Active"
            };

            var created = await _categoryRepo.CreateAsync(category);

            return new CategoryResponseDto
            {
                CategoryId = created.CategoryId,
                Name = created.Name,
                Description = created.Description,
                Status = created.Status
            };
        }
        public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepo.GetAllAsync();
            return categories.Where(c => c.Status == "Active").Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                Description = c.Description,
                Status = c.Status
            }).ToList();
        }

        public async Task<CategoryResponseDto> GetCategoryByIdAsync(Guid id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null || category.Status == "Inactive")
                throw new Exception("Category not found");

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                Status = category.Status
            };
        }

        public async Task<CategoryResponseDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null || category.Status == "Inactive")
                throw new Exception("Category not found");

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                if (dto.Name.Trim().Length > 100)
                    throw new Exception("Category name must not exceed 100 characters");

                var existing = await _categoryRepo.GetByNameAsync(dto.Name.Trim());
                if (existing != null && existing.CategoryId != id)
                    throw new Exception("Category name already exists");
                category.Name = dto.Name.Trim();
            }
            if (!string.IsNullOrWhiteSpace(dto.Description))
                category.Description = dto.Description;

            var updated = await _categoryRepo.UpdateAsync(category);

            return new CategoryResponseDto
            {
                CategoryId = updated.CategoryId,
                Name = updated.Name,
                Description = updated.Description,
                Status = updated.Status
            };
        }

        public async Task<bool> DeleteCategoryAsync(Guid id, bool confirm)
        {
            if (!confirm)
                throw new Exception("Delete confirmation required");

            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
                throw new Exception("Category not found");

            if (category.Products != null && category.Products.Any())
                throw new Exception("Cannot delete category that still has products");

            return await _categoryRepo.DeleteAsync(id);
        }

        public async Task<CategoryResponseDto> UpdateCategoryStatusAsync(Guid id, string status)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
                throw new Exception("Category not found");

            if (status != "Active" && status != "Inactive")
                throw new Exception("Status must be 'Active' or 'Inactive'");

            category.Status = status;
            var updated = await _categoryRepo.UpdateAsync(category);

            return new CategoryResponseDto
            {
                CategoryId = updated.CategoryId,
                Name = updated.Name,
                Description = updated.Description,
                Status = updated.Status
            };
        }
    }
}
