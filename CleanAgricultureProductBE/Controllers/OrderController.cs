using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Order;
using CleanAgricultureProductBE.DTOs.OrderDetail;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Services.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        [Authorize(Roles = "Customer")]
        [HttpGet("me/orders")]
        [SwaggerOperation(Summary = "Lấy danh sách đơn hàng của tôi (Customer)")]
        public async Task<IActionResult> GetAllOrders([FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? keyword)
        {
            var success = "";
            var message = "";

            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var orders = await orderService.GetAllOrders(accountEmail!, page, size, keyword);

            if (orders.ResultObject == null || orders.ResultObject.Count == 0)
            {
                success = "true";
                message = "Không có đơn hàng nào";
            }
            else
            {
                success = "true";
                message = "Lấy thông tin các đơn hàng thành công";
            }

            var response = new ResponseObjectWithPagination<List<OrderResponseDto>>
            {
                Success = success,
                Message = message,
                Data = orders.ResultObject,
                Pagination = orders.Pagination
            };

            return Ok(response);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("me/orders/{orderId}")]
        [SwaggerOperation(Summary = "Lấy chi tiết đơn hàng theo ID (Customer)")]
        public async Task<IActionResult> GetOrderDetail([FromRoute] Guid orderId, [FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? keyword)
        {
            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await orderService.GetOrderDetails(accountEmail!, orderId, page, size, keyword);

            if (result.ResultObject == null)
            {
                return Forbid("Không phải đơn hàng của bạn");
            }

            var response = new ResponseObjectWithPagination<OrderDetailListResponseDto>
            {
                Success = "success",
                Message = "Lấy thông tin đơn hàng thành công",
                Data = result.ResultObject,
                Pagination = result.Pagination
            };

            return Ok(response);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("me/orders")]
        [SwaggerOperation(Summary = "Tạo đơn hàng mới (Customer)")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderRequestDto request)
        {
            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await orderService.PlaceOrder(accountEmail!, request);

            if (result == null)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Không có hàng nào trong vỏ hàng"
                });
            }

            return Ok(new ResponseObject<PlaceOrderResponseDto>
            {
                Success = "success",
                Message = "Đặt hàng thành công",
                Data = result
            });
        }

        [HttpPut("me/orders")]
        [SwaggerOperation(Summary = "Cập nhật đơn hàng (Nothing Here)")]
        public async Task<IActionResult> UpdateOrder()
        {
            return Ok(); 
        }

        [HttpDelete("me/orders")]
        [SwaggerOperation(Summary = "Hủy đơn hàng (Nothing Here)")]
        public async Task<IActionResult> CancelOrder()
        {
            return Ok();
        }
    }
}
