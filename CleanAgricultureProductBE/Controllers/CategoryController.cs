using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/v1/categories")]
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
                return base.Ok(new ResponseObjectWithPagination<List<CategoryResponseDto>>
                {
                    Success = "true",
                    Message = "Lấy danh sách danh mục thành công!",
                    Data = categories,
                    Pagination = null
                });
            }
            catch (Exception ex)
            {
                return base.BadRequest(new DTOs.ApiResponse.ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message,
                });
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
                return base.Ok(new DTOs.ApiResponse.ResponseObject<CategoryResponseDto>
                {
                    Success = "true",
                    Message = "Lấy chi tiết danh mục thành công!",
                    Data = category
                });
            }
            catch (Exception ex)
            {
                return base.NotFound(new DTOs.ApiResponse.ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message,
                });
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
                return base.Ok(new DTOs.ApiResponse.ResponseObject<CategoryResponseDto>
                {
                    Success = "true",
                    Message = "Tạo danh mục thành công!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return base.BadRequest(new DTOs.ApiResponse.ResponseObject<string> 
                { 
                    Success = "false",
                    Message = ex.Message
                });
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
                return base.Ok(new DTOs.ApiResponse.ResponseObject<CategoryResponseDto>
                {
                    Success = "true",
                    Message = "Cập nhật danh mục thành công!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return base.BadRequest(new DTOs.ApiResponse.ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message
                });
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
                return base.Ok(new DTOs.ApiResponse.ResponseObject<bool>
                {
                    Success = "true",
                    Message = "Xóa danh mục thành công!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return base.BadRequest(new DTOs.ApiResponse.ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message
                });
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
                return base.Ok(new DTOs.ApiResponse.ResponseObject<CategoryResponseDto>
                {
                    Success = "true",
                    Message = "Cập nhật trạng thái danh mục thành công!",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return base.BadRequest(new DTOs.ApiResponse.ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message
                });
            }
        }
    }
}
