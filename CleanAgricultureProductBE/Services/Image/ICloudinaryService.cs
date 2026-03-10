namespace CleanAgricultureProductBE.Services.Image
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder);
    }
}
