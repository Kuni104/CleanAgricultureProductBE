using CleanAgricultureProductBE.Services.Schedule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanAgricultureProductBE.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    [ApiController]
    [Route("api/v1/delivery-schedules")]
    public class DeliveryScheduleController : ControllerBase
    {
        private readonly IDeliveryScheduleService _service;

        public DeliveryScheduleController(IDeliveryScheduleService service)
        {
            _service = service;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Lấy danh sách lịch giao hàng (Admin/Staff)")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Lấy chi tiết lịch giao hàng theo ID (Admin/Staff)")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Tạo lịch giao hàng mới (Admin/Staff)")]
        public async Task<IActionResult> Create(Guid orderId, DateTime deliveryTime)
        {
            await _service.CreateAsync(orderId, deliveryTime);
            return Ok("Schedule created");
        }

        [HttpPut("{id}/status")]
        [SwaggerOperation(Summary = "Cập nhật trạng thái lịch giao hàng (Admin/Staff)")]
        public async Task<IActionResult> UpdateStatus(Guid id, string status)
        {
            await _service.UpdateStatusAsync(id, status);
            return Ok("Status updated");
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Xóa lịch giao hàng (Admin/Staff)")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.DeleteAsync(id);
            return Ok("Deleted successfully");
        }
    }
}
