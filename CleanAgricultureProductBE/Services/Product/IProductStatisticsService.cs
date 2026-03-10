using CleanAgricultureProductBE.DTOs.Product;

namespace CleanAgricultureProductBE.Services.Product
{
    public interface IProductStatisticsService
    {
        Task<ProductStatisticsDto> GetProductStatisticsAsync(int? year, int? month, int topN);
    }
}
