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
            // ===== Validate Email =====
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
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Đăng ký tài khoản mới (Customer)")]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Email không được để trống"
                });
            }

            var emailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");

            if (!emailRegex.IsMatch(dto.Email))
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Email không đúng định dạng"
                });
            }

            var existingEmail = await _context.Accounts
                                .AnyAsync(x => x.Email == dto.Email);

            if (existingEmail)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Email đã được sử dụng"
                });
            }

            var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");

            if (!passwordRegex.IsMatch(dto.Password))
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt"
                });
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                dto.PhoneNumber = dto.PhoneNumber.Trim();

                var numberRegex = new Regex(@"^[0-9]+$");

                if (!numberRegex.IsMatch(dto.PhoneNumber))
                {
                    return BadRequest(new ResponseObject<string>
                    {
                        Success = "false",
                        Message = "Số điện thoại chỉ được chứa chữ số"
                    });
                }

                if (dto.PhoneNumber.Length != 10)
                {
                    return BadRequest(new ResponseObject<string>
                    {
                        Success = "false",
                        Message = "Số điện thoại phải có 10 chữ số"
                    });
                }

                var existingPhone = await _context.Accounts
                        .AnyAsync(x => x.PhoneNumber == dto.PhoneNumber);

                if (existingPhone)
                {
                    return BadRequest(new ResponseObject<string>
                    {
                        Success = "false",
                        Message = "Số điện thoại đã được sử dụng"
                    });
                }
            }
            else
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Số điện thoại không được để trống"
                });
            }

            var otpRecord = await _context.EmailOtps
                .Where(x => x.Email == dto.Email && x.IsUsed == false)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpRecord == null)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "Email này chưa gửi OTP"
                });
            }

            if (otpRecord.OtpCode != dto.Otp)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "OTP không đúng"
                });
            }

            if (otpRecord.ExpiredAt < DateTime.UtcNow)
            {
                return BadRequest(new ResponseObject<string>
                {
                    Success = "false",
                    Message = "OTP đã hết hạn"
                });
            }

            try
            {
                var response = await _authService.RegisterAsync(dto);
                otpRecord.IsUsed = true;
                await _context.SaveChangesAsync();

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
