using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services.Image;
using CleanAgricultureProductBE.Services.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductImageService _productImageService;

        public ProductController(IProductService productService, IProductImageService productImageService)
        {
            _productService = productService;
            _productImageService = productImageService;
        }

        // GET: api/Product - Tất cả user có thể xem
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả sản phẩm")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(new ResponseObjectWithPagination<List<ProductResponseDto>>
                {
                    Success = "true",
                    Message = "Lấy các sản phẩm thành công",
                    Data = products,
                    Pagination = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message,
                });
            }
        }

        // GET: api/Product/{id} - Tất cả user có thể xem
        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Lấy chi tiết sản phẩm theo ID")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(new ResponseObject<ProductResponseDto>
                {
                    Success = "true",
                    Message = "Lấy chi tiết sản phẩm thành công",
                    Data = product
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ResponseObject<ProductResponseDto>
                {
                    Success = "false",
                    Message = ex.Message,
                });
            }
        }

        // POST: api/Product - Chỉ Admin và Staff
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [SwaggerOperation(Summary = "Tạo sản phẩm mới (Admin/Staff)")]
        public async Task<IActionResult> CreateProduct(CreateProductDto dto)
        {
            try
            {
                var result = await _productService.CreateProductAsync(dto);
                return Ok(new ResponseObject<ProductResponseDto>
                {
                    Success = "true",
                    Message = "Tạo sản phẩm thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message,
                });
            }
        }

        // PUT: api/Product/{id} - Chỉ Admin và Staff
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        [SwaggerOperation(Summary = "Cập nhật sản phẩm (Admin/Staff)")]
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductDto dto)
        {
            try
            {
                var result = await _productService.UpdateProductAsync(id, dto);
                return Ok(new ResponseObject<ProductResponseDto>
                {
                    Success = "true",
                    Message = "Cập nhật sản phẩm thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message,
                });
            }
        }

        // DELETE: api/Product/{id} - Chỉ Admin
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        [SwaggerOperation(Summary = "Xóa sản phẩm (Admin,Staff)")]
        public async Task<IActionResult> DeleteProduct(Guid id, [FromQuery] bool confirm = false)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id, confirm);
                return Ok(new ResponseObject<bool>
                {
                    Success = "true",
                    Message = result ? "Xóa sản phẩm thành công" : "Sản phẩm đã được xóa tạm thời. Hãy xác nhận xóa vĩnh viễn bằng cách thêm query parameter confirm=true",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string> 
                { 
                    Success = "false", 
                    Message = ex.Message 
                });
            }
        }

        // PUT: api/Product/{id}/status - Chỉ Admin
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Staff")]
        [SwaggerOperation(Summary = "Cập nhật trạng thái sản phẩm (Admin,Staff)")]
        public async Task<IActionResult> UpdateProductStatus(Guid id, [FromBody] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Cần có Status"
                });
            }

            if (status != "Active" && status != "Inactive")
            {
                return BadRequest(new ResponseObject<string> 
                { 
                    Success = "false", 
                    Message = "Status không hợp lệ. (Active hoặc Inactive)" 
                });
            }

            var result = await _productService.UpdateProductStatusAsync(id, status);

            if (!result)
            {
                return NotFound(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Không tìm thấy sản phẩm hoặc cập nhật trạng thái thất bại"
                });
            }

            return Ok(new ResponseObject<bool>
            {
                Success = "true",
                Message = $"Cập nhật trạng thái sản phẩm thành {status} thành công",
                Data = true
            });
        }

        [HttpPost("{productId}/images")]
        [Authorize(Roles = "Admin,Staff")]
        [SwaggerOperation(Summary = "Đăng ảnh sản phẩm (Admin,Staff)")]
        public async Task<IActionResult> UploadProductImages(Guid productId, [FromForm] List<IFormFile> images)
        {
            try
            {
                var result = await _productImageService
                    .UploadProductImagesAsync(productId, images);

                return Ok(new
                {
                    success = true,
                    message = "Upload ảnh sản phẩm thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
