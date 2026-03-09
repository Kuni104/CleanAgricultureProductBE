﻿using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.Services.Schedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanAgricultureProductBE.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [Route("api/v1/schedules")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _service;

        public ScheduleController(IScheduleService service)
        {
            _service = service;
        }

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
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message
                });
            }
        }


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
                    Message = "Gắn đơn thành công"
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
    }
}
