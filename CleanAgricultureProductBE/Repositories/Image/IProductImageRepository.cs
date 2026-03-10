using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories.Image
{
    public interface IProductImageRepository
    {
        Task AddImagesAsync(List<ProductImage> images);
    }
}
