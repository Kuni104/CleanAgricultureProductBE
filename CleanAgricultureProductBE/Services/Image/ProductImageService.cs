using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories.Image;

namespace CleanAgricultureProductBE.Services.Image
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _imageRepo;
        private readonly ICloudinaryService _cloudinaryService;

        private readonly string[] allowedTypes =
        {
            "image/jpeg",
            "image/png",
            "image/jpg",
            "image/webp"
        };

        private const long maxFileSize = 5 * 1024 * 1024; //5MB

        public ProductImageService(
            IProductImageRepository imageRepo,
            ICloudinaryService cloudinaryService)
        {
            _imageRepo = imageRepo;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<List<string>> UploadProductImagesAsync(Guid productId, List<IFormFile> images)
        {
            if (images == null || images.Count == 0)
                throw new Exception("Không có file nào được gửi lên.");

            if (images.Count > 10)
                throw new Exception("Chỉ được upload tối đa 10 ảnh.");

            var imageUrls = new List<string>();
            var imageEntities = new List<ProductImage>();

            foreach (var file in images)
            {
                if (file == null || file.Length == 0)
                    throw new Exception("File không hợp lệ.");

                if (!allowedTypes.Contains(file.ContentType))
                    throw new Exception($"File {file.FileName} không phải là ảnh hợp lệ.");

                if (file.Length > maxFileSize)
                    throw new Exception($"File {file.FileName} vượt quá dung lượng cho phép (5MB).");

                try
                {
                    var imageUrl = await _cloudinaryService.UploadImageAsync(file);

                    imageUrls.Add(imageUrl);

                    imageEntities.Add(new ProductImage
                    {
                        ProductImageId = Guid.NewGuid(),
                        ProductId = productId,

                        Name = file.FileName,
                        Description = "Product image",

                        ImageUrl = imageUrl,

                        Status = "Active"
                    });
                }
                catch
                {
                    throw new Exception($"Upload ảnh {file.FileName} thất bại.");
                }
            }

            await _imageRepo.AddImagesAsync(imageEntities);

            return imageUrls;
        }
    }
}
