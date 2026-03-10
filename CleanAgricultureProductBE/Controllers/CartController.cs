using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Cart;
using CleanAgricultureProductBE.DTOs.CartItem;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace CleanAgricultureProductBE.Controllers
{
    [Authorize(Roles = "Customer")]
    [Route("api/v1")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
        [HttpPost("me/cart/items/{productId}")]
        [SwaggerOperation(Summary = "Thêm sản phẩm vào giỏ hàng")]
        public async Task<IActionResult> AddToCart([FromRoute] Guid productId, [FromBody] CartRequestDto request)
        {
            if(request.Quantity <= 0)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Số lượng phải lớn hơn 0",
                    Data = null
                });
            }

            var success = "";
            var message = "";

            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await cartService.AddToCart(accountEmail!, productId, request);

            if(result.Status == "Stock Error")
            {
                success = "false";
                message = "Thêm vào giỏ hàng không thành công, quá số lượng sản phẩm hiện có";
            }else
            {
                success = "true";
                message = "Thêm vào giỏ hàng thành công";
            }

            var response = new ResponseObject<AddToCartResponseDto>()
            {
                Success = success,
                Message = message,
                Data = result!.Data
            };

            return Ok(response);
        }

        [HttpGet("me/cart")]
        [SwaggerOperation(Summary = "Lấy danh sách sản phẩm trong giỏ hàng")]
        public async Task<IActionResult> GetCartItems([FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? keyword)
        {
            var success = "";
            var message = "";

            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await cartService.GetCartItem(accountEmail!, page, size, keyword);

            var cartItems = new CartItemsResponseWithTotalPriceDto
            {
                CartItemReponseList = result.CartItemsResponseWithTotalPrice?.CartItemReponseList,
                TotalPriceOfAll = result.CartItemsResponseWithTotalPrice?.TotalPriceOfAll
            };

            if (cartItems.CartItemReponseList == null || cartItems.CartItemReponseList.Count == 0)
            {
                success = "true";
                message = "Không có sản phẩm trong giỏ hàng";
            }
            else
            {
                success = "true";
                message = "Lấy giỏ hàng thành công";
            }

            var pagination = result.Pagination == null ? null : result.Pagination;

            var response = new ResponseObjectWithPagination<CartItemsResponseWithTotalPriceDto>()
            {
                Success = success,
                Message = message,
                Data = cartItems,
                Pagination = pagination
            };


            return Ok(response);
        }

        [HttpPatch("me/cart/items/{productId}")]
        [SwaggerOperation(Summary = "Cập nhật số lượng sản phẩm trong giỏ hàng")]
        public async Task<IActionResult> UpdateCartItems([FromRoute] Guid productId, [FromBody] CartRequestDto request)
        {
            if (request.Quantity <= 0)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Số lượng phải lớn hơn 0",
                    Data = null
                });
            }

            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await cartService.UpdateCartItemQuantity(accountEmail!, productId, request);

            if (result.Status == "Stock Error")
            {
                return NotFound(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Cập nhật giỏ hàng không thành công, quá số lượng sản phẩm hiện có",
                    Data = null
                });
            }

            var response = new ResponseObject<UpdateCartResponseDto>
            {
                Success = "true",
                Message = "Cập nhật giỏ hàng thành công",
                Data = result.Data
            };

            return Ok(response);
        }

        [HttpDelete("me/cart/items/{productId}")]
        [SwaggerOperation(Summary = "Xóa một sản phẩm khỏi giỏ hàng")]
        public async Task<IActionResult> DeleteCartItems([FromRoute] Guid productId)
        {
            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await cartService.DeleteCartItem(accountEmail!, productId);

            if (result.Status == "ID 404")
            {

                return NotFound(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Không có sản phẩm trong giỏ hàng",
                    Data = "404"
                });
            }

            var response = new ResponseObject<decimal>
            {
                Success = "true",
                Message = "Xóa sản phẩm khỏi giỏ hàng",
                Data = result.Data
            };

            return Ok(response);
        }

        [HttpDelete("me/cart/items")]
        [SwaggerOperation(Summary = "Xóa tất cả sản phẩm trong giỏ hàng")]
        public async Task<IActionResult> DeleteAllCartItems()
        {
            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await cartService.DeleteAllCartItems(accountEmail!);

            if (result == "404 Cart")
            {
                return NotFound(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "UserId không hợp lệ"
                });
            }

            var response = new ResponseObject<decimal>
            {
                Success = "true",
                Message = "Xóa tất cả sản phẩm trong giỏ hàng thành công",
                Data = 0
            };

            return Ok(response);
        }

    }
}
