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

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader))
                return BadRequest(new { message = "Authorization header missing" });

            var token = authHeader.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase)
                ? authHeader.Substring("Bearer".Length).Trim()
                : authHeader.Trim();

            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "Bearer token missing" });

            try
            {
                await _authService.LogoutAsync(token);
                return NoContent();
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Unable to logout at this time" });
            }
        }
    }
}