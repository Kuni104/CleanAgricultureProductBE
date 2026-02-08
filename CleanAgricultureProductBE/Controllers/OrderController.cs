using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        [HttpGet("/me")]
        public async Task<IActionResult> GetAllOrders()
        {
            return Ok();
        }

        [HttpPost("/me")]
        public async Task<IActionResult> PlaceOrder()
        {
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrder()
        {
            return Ok(); 
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOrder()
        {
            return Ok();
        }
    }
}
