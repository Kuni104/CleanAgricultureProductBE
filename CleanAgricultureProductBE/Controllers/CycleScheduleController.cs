using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.CycleSchedule;
using CleanAgricultureProductBE.Services.CycleSchedule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/cycle-schedule")]
    [ApiController]
    public class CycleScheduleController(ICycleScheduleService cycleScheduleService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCycleSchedules()
        {
            var message = "";
            var cycleSchedules = await cycleScheduleService.GetCycleSchedules();

            if (cycleSchedules == null)
            {
                message = "Không có lịch xoay tua nào";
            }
            else
            {
                message = "Lấy lịch xoay tua thành công";
            }

            return Ok(new ResponseObject<List<CycleScheduleResponseDto>>
            {
                Success = "true",
                Message = message,
                Data = cycleSchedules
            });
        }
    }
}
