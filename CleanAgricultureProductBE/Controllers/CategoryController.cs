using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.Services.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Category - Tất cả user có thể xem
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Lấy danh sách tất cả danh mục")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/Category/{id} - Tất cả user có thể xem
        [HttpGet("{id}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Lấy chi tiết danh mục theo ID")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                return Ok(category);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/Category - Chỉ Admin và Staff
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [SwaggerOperation(Summary = "Tạo danh mục mới (Admin/Staff)")]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
        {
            try
            {
                var result = await _categoryService.CreateCategoryAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/Category/{id} - Chỉ Admin và Staff
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        [SwaggerOperation(Summary = "Cập nhật danh mục (Admin/Staff)")]
        public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryDto dto)
        {
            try
            {
                var result = await _categoryService.UpdateCategoryAsync(id, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Category/{id} - Chỉ Admin
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Xóa danh mục (Admin)")]
        public async Task<IActionResult> DeleteCategory(Guid id, [FromQuery] bool confirm = false)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id, confirm);
                return Ok(new { message = "Category deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH: api/Category/{id}/status - Chỉ Admin
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Cập nhật trạng thái danh mục (Admin)")]
        public async Task<IActionResult> UpdateCategoryStatus(Guid id, [FromBody] string status)
        {
            try
            {
                var result = await _categoryService.UpdateCategoryStatusAsync(id, status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
