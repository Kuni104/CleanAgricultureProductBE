using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Google;
using CleanAgricultureProductBE.Services.Auth;
using CleanAgricultureProductBE.Services.OTP;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public AuthController(IEmailOtpService otpService, IConfiguration configuration, IAuthService authService)
        {
            _otpService = otpService;
            _configuration = configuration;
            _authService = authService;
        }

        [HttpPost("send-otp")]
        [SwaggerOperation(Summary = "Gửi OTP qua Email")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Email không được để trống"
                });
            }

            var emailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");

            if (!emailRegex.IsMatch(request.Email))
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Email không đúng định dạng"
                });
            }

            try
            {
                await _otpService.SendOtpAsync(request.Email);

                return Ok(new ResponseObject<string>
                {
                    Success = "true",
                    Message = "Gửi OTP thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "OTP hết hạn hoặc không khả dụng"
                });
            }
        }

        [HttpPost("test/verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var result = await _otpService.VerifyOtpAsync(request.Email, request.OtpCode);

            if (!result)
                return BadRequest("OTP hết hạn hoặc không khả dụng");

            return Ok("Xác minh OTP thành công");
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
                    Success = "true",
                    Message = "Đăng nhập thành công",
                    Data = result
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return Unauthorized(new ResponseObject<LoginResponseDto>
                {
                    Success = "false",
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Đăng ký tài khoản mới (Customer)")]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            try
            {
                await _authService.ValidateRegisterAsync(dto);
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
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = ex.Message
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
                    Message = "Phải có token"
                });

            await _authService.LogoutAsync(token);

            return Ok(new ResponseObject<string>
            {
                Success = "true",
                Message = "Đăng xuất thành công"
            });
        }

        [HttpPost("forgotpassword")]
        [SwaggerOperation(Summary = "Đặt lại mật khẩu bằng Email")]
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
        [SwaggerOperation(Summary = "Đặt lại mật khẩu từ mật khẩu cũ")]
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

        [HttpPost("google-login")]
        [SwaggerOperation(Summary = "Đăng nhập bằng Google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto request)
        {
            try
            {
                var result = await _authService.LoginWithGoogleAsync(request.IdToken);

                return Ok(new ResponseObject<LoginResponseDto>
                {
                    Success = "true",
                    Message = "Đăng nhập Google thành công",
                    Data = result
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
