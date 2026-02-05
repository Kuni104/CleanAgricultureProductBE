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
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
        {
            var accountEmail = User.FindFirstValue(ClaimTypes.Email);

            var result = await cartService.AddToCart(accountEmail!, request);

            var response = new ResponseObject<AddToCartResponseDto>()
            {
                Success = "true",
                Message = "Product added to cart successfully",
                Data = result
            };

            return Ok(response);
        }
    }
}
