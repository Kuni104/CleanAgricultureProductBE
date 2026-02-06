using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Product - Tất cả user có thể xem
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Product/{id} - Tất cả user có thể xem
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/Product - Chỉ Admin và Staff
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CreateProduct(CreateProductDto dto)
        {
            try
            {
                var result = await _productService.CreateProductAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Product/{id} - Chỉ Admin và Staff
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductDto dto)
        {
            try
            {
                var result = await _productService.UpdateProductAsync(id, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Product/{id} - Chỉ Admin
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(Guid id, [FromQuery] bool confirm = false)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id, confirm);
                return Ok(new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Product/{id}/status - Chỉ Admin
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProductStatus(Guid id, [FromBody] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest(new { message = "Status is required" });
            }

            if (status != "Active" && status != "Inactive")
            {
                return BadRequest(new { message = "Invalid status. Must be 'Active' or 'Inactive'" });
            }

            var result = await _productService.UpdateProductStatusAsync(id, status);

            if (!result)
            {
                return NotFound(new { message = "Product not found" });
            }

            return Ok(new { message = $"Product status updated to {status} successfully" });
        }
    }
}
