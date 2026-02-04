using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories;

namespace CleanAgricultureProductBE.Services
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
            var category = new Category
            {
                CategoryId = Guid.NewGuid(),
                Name = dto.Name,
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
    }
}
