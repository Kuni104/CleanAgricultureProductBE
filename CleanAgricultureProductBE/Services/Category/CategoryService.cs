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
                throw new Exception("Tên danh mục không được để trống");

            if (dto.Name.Trim().Length > 100)
                throw new Exception("Tên danh mục không được vượt quá 100 ký tự");

            var existing = await _categoryRepo.GetByNameAsync(dto.Name.Trim());
            if (existing != null)
                throw new Exception("Tên danh mục đã tồn tại");

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
                throw new Exception("Không tìm thấy danh mục");

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
                throw new Exception("Không tìm thấy danh mục");

            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                if (dto.Name.Trim().Length > 100)
                    throw new Exception("Tên danh mục không được vượt quá 100 ký tự");

                var existing = await _categoryRepo.GetByNameAsync(dto.Name.Trim());
                if (existing != null && existing.CategoryId != id)
                    throw new Exception("Tên danh mục đã tồn tại");
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
                throw new Exception("Yêu cầu xác nhận xóa");

            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
                throw new Exception("Không tìm thấy danh mục");

            if (category.Products != null && category.Products.Any())
                throw new Exception("Không thể xóa danh mục đang có sản phẩm");

            return await _categoryRepo.DeleteAsync(id);
        }

        public async Task<CategoryResponseDto> UpdateCategoryStatusAsync(Guid id, string status)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
                throw new Exception("Không tìm thấy danh mục");

            if (status != "Active" && status != "Inactive")
                throw new Exception("Trạng thái phải là 'Active' hoặc 'Inactive'");

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
