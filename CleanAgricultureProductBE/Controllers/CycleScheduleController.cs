using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.CycleSchedule;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services.CycleSchedule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/cycle-schedule")]
    [ApiController]
    public class CycleScheduleController(ICycleScheduleService cycleScheduleService) : ControllerBase
    {

        [HttpGet]
        [SwaggerOperation(Summary = "Lấy lịch xoay tua")]
        public async Task<IActionResult> GetCycleSchedules([FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? keyword)
        {
            try
            {
            var message = "";
            var result = await cycleScheduleService.GetCycleSchedules(page, size, keyword);

            if (result == null)
            {
                message = "Không có lịch xoay tua nào";
            }
            else
            {
                message = "Lấy lịch xoay tua thành công";
            }

            return Ok(new ResponseObjectWithPagination<List<CycleScheduleResponseDto>>
            {
                Success = "true",
                Message = message,
                Data = result.ResultObject,
                Pagination = result.Pagination
            });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string> { Success = "false", Message = ex.Message });
            }
        }
    }
}
