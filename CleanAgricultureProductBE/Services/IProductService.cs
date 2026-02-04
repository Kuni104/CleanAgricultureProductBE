using CleanAgricultureProductBE.DTOs;

namespace CleanAgricultureProductBE.Services
{
    public interface IProductService
    {
        Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto);
    }
}
