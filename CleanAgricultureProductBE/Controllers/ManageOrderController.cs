using CleanAgricultureProductBE.DTOs.Order;
using CleanAgricultureProductBE.DTOs.OrderDetail;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/v1/admin/orders")]
    [ApiController]
    public class ManageOrderController(IOrderService orderService) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? keyword)
        {
            var success = "";
            var message = "";

            var orders = await orderService.GetAllOrdersAdmin(page, size, keyword);

            if (orders.ResultObject == null || orders.ResultObject.Count == 0)
            {
                success = "true";
                message = "Không có đơn hàng nào";
            }
            else
            {
                success = "true";
                message = "Lấy tất cả đơn hàng thành công!";
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

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetails([FromRoute] Guid orderId, [FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? keyword)
        {
            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await orderService.GetOrderDetailsAdmin(orderId, page, size, keyword);
            if (result.ResultObject == null)
            {
                var responseN = new ResponseObject<OrderDetailListResponseDto>
                {
                    Success = "true",
                    Message = "Lấy chi tiết đơn hàng thành công",
                    Data = result.ResultObject,
                    Pagination = result.Pagination
                };

                return NotFound(responseN);
            }

            var response = new ResponseObject<OrderDetailListResponseDto>
            {
                Success = "true",
                Message = "Lấy chi tiết đơn hàng thành công",
                Data = result.ResultObject,
                Pagination = result.Pagination
            };

            return Ok(response);
        }


        [Authorize(Roles = "Admin,Staff,DeliveryPerson")]
        [HttpPatch("{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] Guid orderId, [FromBody] UpdateOrderStatusRequestDto request)
        {
            var success = "";
            var message = "";

            var result = await orderService.UpdateOrderStatus(orderId, request);
            if (result == null)
            {
                success = "false";
                message = "Cập nhật trạng thái đơn hàng thất bại!";
            }
            else
            {
                success = "true";
                message = "Cập nhật trạng thái đơn hàng thành công!";
            }

            var response = new ResponseObject<object>
            {
                Success = success,
                Message = message,
                Data = result
            };
            return Ok(response);
        }
    }
}
