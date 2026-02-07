using CleanAgricultureProductBE.DTOs;
namespace CleanAgricultureProductBE.Services
{
    public interface IProductService
    {
        Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto);
        Task<List<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto> GetProductByIdAsync(Guid id);
        Task<ProductResponseDto> UpdateProductAsync(Guid id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(Guid id, bool confirm);
        Task<bool> UpdateProductStatusAsync(Guid productId, string status);
    }
}
