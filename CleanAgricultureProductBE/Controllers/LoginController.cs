using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.OTP;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/v1/auth")]
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

        [HttpPost("login")]
        [SwaggerOperation(Summary = "Đăng nhập và nhận JWT token")]
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
                return Unauthorized(new ResponseObject<LoginResponseDto>
                {
                    Success = "error",
                    Message = "Sai Tài Khoản Hoặc Mật Khẩu",
                    Data = null
                });
            }
        }

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Đăng ký tài khoản mới")]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var response = await _authService.RegisterAsync(dto);
                return Created(string.Empty, new ResponseObject<LoginResponseDto>
                {
                    Success = "true",
                    Message = "Đăng kí thành công",
                    Data = response
                });
            }   catch(Exception ex)
            {
                if (ex.Message.Contains("Email already in use", StringComparison.OrdinalIgnoreCase))
                    return Conflict(new ResponseObject<string>
                    {
                        Success = "false",
                        Message = ex.Message,
                    });
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message,
                });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        [SwaggerOperation(Summary = "Đăng xuất và vô hiệu hóa token")]
        public async Task<IActionResult> Logout()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Phải có token",
                });

            await _authService.LogoutAsync(token);

            return NoContent();
        }

        [HttpPost("request-reset-password")]
        [SwaggerOperation(Summary = "Yêu cầu đặt lại mật khẩu - gửi OTP qua email")]
        public async Task<IActionResult> RequestResetPassword(RequestResetPasswordDto dto)
        {
            await _authService.RequestResetPasswordAsync(dto.Email);
            return Ok("OTP sent to email");
        }

        [HttpPost("reset-password")]
        [SwaggerOperation(Summary = "Xác nhận OTP và đặt lại mật khẩu mới")]
        public async Task<IActionResult> ResetPassword(ConfirmResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return Ok("Password reset successfully");
        }
    }
}
