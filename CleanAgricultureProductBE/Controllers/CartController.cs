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
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
        [HttpPost("me/items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
        {
            var success = "";
            var message = "";

            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await cartService.AddToCart(accountEmail!, request);

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

        [HttpGet("me")]
        public async Task<IActionResult> GetCartItems([FromQuery] int? page, [FromQuery] int? size, [FromQuery] string? keyword)
        {
            var success = "";
            var message = "";

            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await cartService.GetCartItem(accountEmail!, page, size, keyword);

            var cartItems = new GetCartItemsResponseWithTotalPrice
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

            var response = new ResponseObject<GetCartItemsResponseWithTotalPrice>()
            {
                Success = success,
                Message = message,
                Data = cartItems,
                Pagination = pagination
            };


            return Ok(response);
        }

        [HttpPut("me/items")]
        public async Task<IActionResult> UpdateCartItems([FromBody] UpdateCartItemRequestDto request)
        {
            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await cartService.UpdateCartItem(accountEmail!, request);

            var response = new ResponseObject<GetCartItemReponseDto>
            {
                Success = "true",
                Message = "Updated",
                Data = result,
            };

            return Ok(response);
        }
    }
}
