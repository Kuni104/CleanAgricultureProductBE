using CleanAgricultureProductBE.Services.Schedule;
using Microsoft.AspNetCore.Mvc;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _service;

        public ScheduleController(IScheduleService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid deliveryPersonId, DateTime scheduledDate)
        {
            var id = await _service.CreateScheduleAsync(deliveryPersonId, scheduledDate);
            return Ok(new { ScheduleId = id });
        }

        [HttpPost("assign-orders")]
        public async Task<IActionResult> AssignOrders(Guid scheduleId, List<Guid> orderIds)
        {
            await _service.AssignOrdersAsync(scheduleId, orderIds);
            return Ok("Orders assigned successfully");
        }
    }
}
