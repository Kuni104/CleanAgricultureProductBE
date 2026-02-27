using CleanAgricultureProductBE.Services.Schedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanAgricultureProductBE.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryScheduleController : ControllerBase
    {
        private readonly IDeliveryScheduleService _service;

        public DeliveryScheduleController(IDeliveryScheduleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid orderId, DateTime deliveryTime)
        {
            await _service.CreateAsync(orderId, deliveryTime);
            return Ok("Schedule created");
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, string status)
        {
            await _service.UpdateStatusAsync(id, status);
            return Ok("Status updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok("Deleted successfully");
        }
    }
}
