using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.Services;
using Microsoft.AspNetCore.Mvc;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context,IConfiguration configuration ,IAuthService authService)
        {
            _context = context;
            _configuration = configuration;
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var response = await _authService.RegisterAsync(dto);
                return Created(string.Empty, response);
            }   catch(Exception ex)
            {
                if (ex.Message.Contains("Email already in use", StringComparison.OrdinalIgnoreCase))
                    return Conflict(new { message = ex.Message });
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}