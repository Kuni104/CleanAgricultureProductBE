using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // API KHÔNG CẦN LOGIN
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("Public endpoint works");
        }

        // API BẮT BUỘC LOGIN
        [Authorize]
        [HttpGet("protected")]
        public IActionResult Protected()
        {
            return Ok("JWT Authorized OK");
        }

        // API REQUIRE ROLE
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminOnly()
        {
            return Ok("Admin access OK");
        }
    }
}
