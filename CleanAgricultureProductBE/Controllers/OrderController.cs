using CleanAgricultureProductBE.DTOs.Order;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Models;
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
        public async Task<IActionResult> GetAllOrders([FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? keyword)
        {
            var success = "";
            var message = "";

            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var orders = await orderService.GetAllOrders(accountEmail!, page, size, keyword);

            if (orders.ResultObject == null || orders.ResultObject.Count == 0)
            {
                success = "true";
                message = "No items in cart";
            }
            else
            {
                success = "true";
                message = "Cart items retrieved successfully";
            }

            var response = new ResponseObject<List<OrderResponseDto>>
            {
                Success = success,
                Message = message,
                Data = orders.ResultObject,
                Pagination = orders.Pagination
            };

            return Ok(response);
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
