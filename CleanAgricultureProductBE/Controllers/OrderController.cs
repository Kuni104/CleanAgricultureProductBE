using CleanAgricultureProductBE.DTOs.Order;
using CleanAgricultureProductBE.Services.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        [Authorize(Roles = "Customer")]
        [HttpGet("/me")]
        public async Task<IActionResult> GetAllOrders()
        {
            return Ok();
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("/me")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderRequestDto request)
        {
            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await orderService.PlaceOrder(accountEmail!, request);

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrder()
        {
            return Ok(); 
        }

        [HttpDelete]
        public async Task<IActionResult> CancelOrder()
        {
            return Ok();
        }
    }
}
