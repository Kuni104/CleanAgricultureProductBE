using CleanAgricultureProductBE.DTOs.Cart;
using CleanAgricultureProductBE.DTOs.CartItem;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanAgricultureProductBE.Controllers
{
    [Authorize(Roles = "Customer")]
    [Route("api")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
        [HttpPost("me/cart/items/{productId}")]
        public async Task<IActionResult> AddToCart([FromRoute] Guid productId, [FromBody] CartRequestDto request)
        {
            var success = "";
            var message = "";

            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await cartService.AddToCart(accountEmail!, productId, request);

            if(result == null)
            {
                success = "false";
                message = "Failed to add product to cart";
            }else
            {
                success = "true";
                message = "Product added to cart successfully";
            }

            var response = new ResponseObject<AddToCartResponseDto>()
            {
                Success = success,
                Message = message,
                Data = result
            };

            return Ok(response);
        }

        [HttpGet("me/cart")]
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
                message = "No items in cart";
            }
            else
            {
                success = "true";
                message = "Cart items retrieved successfully";
            }

            var pagination = result.Pagination == null ? null : result.Pagination;

            var response = new ResponseObject<CartItemsResponseWithTotalPriceDto>()
            {
                Success = success,
                Message = message,
                Data = cartItems,
                Pagination = pagination
            };


            return Ok(response);
        }

        [HttpPatch("me/cart/items/{productId}")]
        public async Task<IActionResult> UpdateCartItems([FromRoute] Guid productId, [FromBody] CartRequestDto request)
        {
            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await cartService.UpdateCartItemQuantity(accountEmail!, productId, request);

            var response = new ResponseObject<UpdateCartResponseDto>
            {
                Success = "true",
                Message = "Updated",
                Data = result,
            };

            return Ok(response);
        }

        [HttpDelete("me/cart/items/{productId}")]
        public async Task<IActionResult> DeleteCartItems([FromRoute] Guid productId)
        {
            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await cartService.DeleteCartItem(accountEmail!, productId);

            if (result.Status == "ID 404")
            {

                return NotFound(new ResponseObject<string>
                {
                    Success = "fail",
                    Message = "Product Not Found",
                    Data = "404"
                });
            }

            var response = new ResponseObject<decimal>
            {
                Success = "true",
                Message = "Remove Item From Cart And Return New Total Price Of Cart",
                Data = result.Data
            };

            return Ok(response);
        }

        [HttpDelete("me/cart/items")]
        public async Task<IActionResult> DeleteAllCartItems()
        {
            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await cartService.DeleteAllCartItems(accountEmail!);

            if (result == "404 Cart")
            {
                return NotFound(new ResponseObject<string>
                {
                    Success = "fail",
                    Message = "Invalid User ID"
                });
            }

            var response = new ResponseObject<decimal>
            {
                Success = "true",
                Message = "Remove All Items From Cart Successful",
                Data = 0
            };

            return Ok(response);
        }

    }
}
