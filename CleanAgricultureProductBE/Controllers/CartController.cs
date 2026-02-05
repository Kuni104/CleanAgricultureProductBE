using CleanAgricultureProductBE.DTOs.Cart;
using CleanAgricultureProductBE.Services.Cart;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequestDto request)
        {
            string response = request.ProductId.ToString() + " " + request.Quantity;
            return Ok(response);
        }
    }
}
