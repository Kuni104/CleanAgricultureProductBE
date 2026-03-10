using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.DTOs.Schedule;
using CleanAgricultureProductBE.Services.Schedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/v1/schedules")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _service;

        public ScheduleController(IScheduleService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [SwaggerOperation(Summary = "Tạo lịch giao hàng")]
        public async Task<IActionResult> Create([FromBody] CreateScheduleRequestDto dto)
        {
            try
            {
                var id = await _service.CreateScheduleAsync(dto.DeliveryPersonId, dto.ScheduledDate);

                return Ok(new ResponseObject<Guid>
                {
                    Success = "true",
                    Message = "Tạo lịch thành công",
                    Data = id
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<Guid>
                {
                    Success = "false",
                    Message = ex.Message,
                    Data = Guid.Empty
                });
            }
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost("assign-orders")]
        [SwaggerOperation(Summary = "Gán đơn hàng vào lịch giao hàng")]
        public async Task<IActionResult> AssignOrders([FromBody] AssignOrdersRequestDto dto)
        {
            try
            {
                await _service.AssignOrdersAsync(dto.ScheduleId, dto.OrderIds);

                return Ok(new ResponseObject<string>
                {
                    Success = "true",
                    Message = "Gắn đơn thành công",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [Authorize(Roles = "DeliveryPerson")]
        [HttpGet("delivery-person-schedules")]
        [SwaggerOperation(Summary = "Lấy tất cả lịch giao hàng của người vận chuyển")]
        public async Task<IActionResult> GetAllScheduleOfDeliveryPerson([FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? keyword)
        {
            var success = "";
            var message = "";

            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var schedules = await _service.GetSchedulesOfDeliveryPerson(accountEmail!, page, size, keyword);

            if (schedules.ResultObject == null || schedules.ResultObject.Count <= 0)
            {
                success = "true";
                message = "Không có lịch nào";
            }
            else
            {
                success = "true";
                message = "Lấy lịch thành công";
            }

            return Ok(new ResponseObjectWithPagination<List<ScheduleResponseDto>>
            {
                Success = success,
                Message = message,
                Data = schedules.ResultObject,
                Pagination = schedules.Pagination
            });
        }
    }
}
