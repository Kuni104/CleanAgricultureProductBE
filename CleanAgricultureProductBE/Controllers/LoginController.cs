using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.DTOs.OTP;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
                var apiResponse = new ResponseObject<LoginResponseDto>
                {
                    Success = "success",
                    Message = "Login successful",
                    Data = (LoginResponseDto) result
                };

                return Ok(apiResponse);
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

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "Token not found" });

            await _authService.LogoutAsync(token);

            return NoContent();
        }

        [HttpPost("request-reset-password")]
        public async Task<IActionResult> RequestResetPassword(RequestResetPasswordDto dto)
        {
            await _authService.RequestResetPasswordAsync(dto.Email);
            return Ok("OTP sent to email");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ConfirmResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return Ok("Password reset successfully");
        }
    }
}