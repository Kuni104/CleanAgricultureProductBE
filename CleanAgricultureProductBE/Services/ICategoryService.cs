using CleanAgricultureProductBE.DTOs;

namespace CleanAgricultureProductBE.Services
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto);
    }
}
