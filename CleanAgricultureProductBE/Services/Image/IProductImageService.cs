namespace CleanAgricultureProductBE.Services.Image
{
    public interface IProductImageService
    {
        Task<List<string>> UploadProductImagesAsync(Guid productId, List<IFormFile> imagegs);
    }
}
