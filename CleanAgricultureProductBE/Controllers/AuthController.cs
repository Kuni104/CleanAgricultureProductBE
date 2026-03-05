using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Services;
using CleanAgricultureProductBE.Services.OTP;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            await _otpService.SendOtpAsync(request.Email);
            return Ok("OTP sent successfully");
        }

        [HttpPost("verify-otp")]
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
                    Message = "Login successful",
                    Data = (LoginResponseDto)result
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
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
    }

    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }

    public class SendOtpRequest
    {
        public string Email { get; set; }
    }
}
