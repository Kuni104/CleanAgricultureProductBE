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
        [SwaggerOperation(Summary = "Tạo lịch giao hàng (Format: YYYY-MM-DDTHH:mm:ss.fffZ | 2026-03-1T17:30:45.123Z)")]
        public async Task<IActionResult> Create([FromBody] CreateScheduleRequestDto dto)
        {
            try
            {
                var id = await _service.CreateScheduleAsync(dto.DeliveryPersonId, dto.ScheduledDate);

                return Ok(new ResponseObject<CreateScheduleResponseDto>
                {
                    Success = "true",
                    Message = "Tạo lịch thành công",
                    Data = new CreateScheduleResponseDto
                    {
                        ScheduleId = id
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<CreateScheduleResponseDto>
                {
                    Success = "false",
                    Message = ex.Message,
                    Data = null
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
        [HttpGet("delivery-person")]
        [SwaggerOperation(Summary = "Lấy tất cả lịch giao hàng của người vận chuyển (DeliveryPerson)")]
        public async Task<IActionResult> GetAllScheduleOfDeliveryPerson([FromQuery] int? page, [FromQuery] int? size)
        {
            var success = "";
            var message = "";

            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var keyword = "";

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

        [Authorize(Roles = "DeliveryPerson")]
        [HttpGet("delivery-person/today")]
        [SwaggerOperation(Summary = "Lấy lịch giao hàng của người vận chuyển hôm nay (DeliveryPerson)")]
        public async Task<IActionResult> GetAllScheduleOfDeliveryPersonToday()
        {
            var success = "";
            var message = "";

            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var schedule = await _service.GetSchedulesOfDeliveryPersonToday(accountEmail!);

            if (schedule == null)
            {
                success = "true";
                message = "Không có lịch nào vào ngày hôm nay";
            }
            else
            {
                success = "true";
                message = "Lấy lịch thành công";
            }

            return Ok(new ResponseObject<ScheduleResponseDto>
            {
                Success = success,
                Message = message,
                Data = schedule
            });
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("today")]
        [SwaggerOperation(Summary = "Lấy tất cả lịch giao hàng hôm nay (Admin/Staff)")]
        public async Task<IActionResult> GetAllSchedulesTodayAdmin([FromQuery] int? page, [FromQuery] int? size)
        {
            var success = "";
            var message = "";

            var schedules = await _service.GetSchedulesToday(page, size);

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

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("today/{deliveryPersonId}")]
        [SwaggerOperation(Summary = "Lấy lịch giao hàng hôm nay theo người vận chuyển (Admin/Staff)")]
        public async Task<IActionResult> GetAllScheduleOfDeliveryPersonTodayAdmin([FromRoute] Guid deliveryPersonId)
        {
            var success = "";
            var message = "";

            var result = await _service.GetSchedulesTodayByDeliveryPerson(deliveryPersonId);

            if (result.Status == "Schedule 404")
            {
                success = "true";
                message = $"Không có lịch nào vào ngày hôm nay của người vận chuyển với ID: {deliveryPersonId}";
            }else if (result.Status == "Account 404")
            {
                success = "false";
                message = $"Không có người vận chuyển với ID: {deliveryPersonId}";
                return BadRequest(new ResponseObject<string>
                {
                    Success = success,
                    Message = message,
                });
            }
            else
            {
                success = "true";
                message = "Lấy lịch thành công";
            }

            return Ok(new ResponseObject<ScheduleResponseDto>
            {
                Success = success,
                Message = message,
                Data = result!.Data
            });
        }
    }
}
