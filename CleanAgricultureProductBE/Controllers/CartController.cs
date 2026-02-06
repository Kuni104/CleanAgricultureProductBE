using CleanAgricultureProductBE.DTOs.Cart;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanAgricultureProductBE.Controllers
{
    [Authorize(Roles = "Customer")]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
        [HttpPost("me")]
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
        public async Task<IActionResult> GetCartItems([FromQuery] int page, [FromQuery] int size, [FromQuery] string keyword)
        {
 
            return Ok();
        }
    }
}
