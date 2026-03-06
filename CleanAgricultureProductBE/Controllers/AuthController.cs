using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.Services;
using CleanAgricultureProductBE.Services.OTP;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace CleanAgricultureProductBE.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IEmailOtpService _otpService;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(IEmailOtpService otpService, AppDbContext context, IConfiguration configuration, IAuthService authService)
        {
            _otpService = otpService;
            _context = context;
            _configuration = configuration;
            _authService = authService;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpDto request)
        {
            await _otpService.SendOtpAsync(request.Email);
            return Ok("OTP sent successfully");
        }

        [HttpPost("test/verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var result = await _otpService.VerifyOtpAsync(request.Email, request.OtpCode);

            if (!result)
                return BadRequest("Invalid or expired OTP");

            return Ok("OTP verified successfully");
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
                    Message = "Đăng nhập thành công",
                    Data = result
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
            if (dto.PhoneNumber == null || dto.PhoneNumber.Trim() == "")
            {

            }
            else
            {
                var isValidPhoneNumber = Regex.IsMatch(dto.PhoneNumber, @"^(?:\+84|0)\d{9}$");
                if (!isValidPhoneNumber)
                {
                    return BadRequest(new ResponseObject<string>
                    {
                        Success = "false",
                        Message = "Số điện thoại không hợp lệ! Số điện thoại phải bắt đầu bằng 0 và có 10 chữ số.",
                    });
                }
            }

            try
            {
                var response = await _authService.RegisterAsync(dto);
                return Created(string.Empty, new ResponseObject<LoginResponseDto>
                {
                    Success = "true",
                    Message = "Đăng kí thành công",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Email đã được sử dụng", StringComparison.OrdinalIgnoreCase))
                    return Conflict(new ResponseObject<string>
                    {
                        Success = "false",
                        Message = ex.Message,
                    });
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "ex.Message",
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

        [HttpPost("forgotpassword")]
        [SwaggerOperation(Summary = "Đặt lại mật khẩu bằng OTP")]
        public async Task<IActionResult> ForgotPassword([FromBody] FotgotPasswordDto dto)
        {
            try
            {
                await _authService.ForgotPasswordAsync(dto);

                return Ok(new ResponseObject<string>
                {
                    Success = "true",
                    Message = "Đổi mật khẩu thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                    return Unauthorized();

                await _authService.ChangePasswordAsync(Guid.Parse(userId), dto);

                return Ok(new ResponseObject<string>
                {
                    Success = "true",
                    Message = "Đổi mật khẩu thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message
                });
            }
        }

        public class VerifyOtpRequest
        {
            public string Email { get; set; }
            public string OtpCode { get; set; }
        }
    }
}
