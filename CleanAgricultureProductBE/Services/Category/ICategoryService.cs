using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.DTOs.ApiResponse;

namespace CleanAgricultureProductBE.Services.Category
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<CategoryResponseDto> GetCategoryByIdAsync(Guid id);
        Task<CategoryResponseDto> UpdateCategoryAsync(Guid id, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(Guid id, bool confirm);
        Task<CategoryResponseDto> UpdateCategoryStatusAsync(Guid id, string status);
        Task<ResponseDtoWithPagination<List<CategoryResponseDto>>> GetAllCategoriesWithPaginationAsync(int? page, int? size, string? status);
    }
}
