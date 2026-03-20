using CleanAgricultureProductBE.Enum;
using ProductModel = CleanAgricultureProductBE.Models.Product;

namespace CleanAgricultureProductBE.Repositories.Product
{
    public interface IProductRepository
    {
        Task<ProductModel> CreateAsync(ProductModel product);
        Task<List<ProductModel>> GetAllAsync();
        Task<ProductModel?> GetByIdAsync(Guid id);
        Task<ProductModel> UpdateAsync(ProductModel product);
        Task<bool> DeleteAsync(Guid id);
        Task<List<ProductModel>> GetAllWithPaginationAsync(int offset, int pageSize, Guid? categoryId, string? keyword, decimal? minPrice, decimal? maxPrice, ProductStatusEnum productStatus);
        Task<int> CountAllAsync(Guid? categoryId, string? keyword, decimal? minPrice, decimal? maxPrice, ProductStatusEnum productStatus);
    }
}
