using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Product;
using CleanAgricultureProductBE.Services.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/v1/product-statistics")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class ProductStatisticsController : ControllerBase
    {
        private readonly IProductStatisticsService _statisticsService;

        public ProductStatisticsController(IProductStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lấy báo cáo thống kê sản phẩm (Admin/Staff)")]
        public async Task<IActionResult> GetProductStatistics(
            [FromQuery] int? year,
            [FromQuery] int? month,
            [FromQuery] int topN = 10)
        {
            try
            {
                var statistics = await _statisticsService.GetProductStatisticsAsync(year, month, topN);
                return Ok(new ResponseObject<ProductStatisticsDto>
                {
                    Success = "true",
                    Message = "Lấy báo cáo thống kê sản phẩm thành công",
                    Data = statistics
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
    }
}
