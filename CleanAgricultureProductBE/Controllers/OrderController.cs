using CleanAgricultureProductBE.DTOs.Order;
using CleanAgricultureProductBE.Services.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api")]
    [ApiController]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        [Authorize(Roles = "Customer")]
        [HttpGet("me/orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            return Ok();
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("me/orders")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderRequestDto request)
        {
            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await orderService.PlaceOrder(accountEmail!, request);

            return Ok(result);
        }

        [HttpPut("me/orders")]
        public async Task<IActionResult> UpdateOrder()
        {
            return Ok(); 
        }

        [HttpDelete("me/orders")]
        public async Task<IActionResult> CancelOrder()
        {
            return Ok();
        }
    }
}
