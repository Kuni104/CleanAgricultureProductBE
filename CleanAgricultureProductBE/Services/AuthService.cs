using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.DTOs.OTP;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Repositories.Cart;
using CleanAgricultureProductBE.Repositories.OTP;
using CleanAgricultureProductBE.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CleanAgricultureProductBE.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IConfiguration _config;
        private readonly ITokenBlacklistRepository _tokenBlacklistRepo;
        private readonly IPasswordResetOtpRepository _otpRepo;
        private readonly IEmailService _emailService;

        public AuthService(
            IAccountRepository accountRepo,
            ICartRepository cartRepo,
            IConfiguration config,
            ITokenBlacklistRepository tokenBlacklistRepo,
            IPasswordResetOtpRepository otpRepo,
            IEmailService emailService)
        {
            _accountRepo = accountRepo;
            _cartRepo = cartRepo;
            _config = config;
            _tokenBlacklistRepo = tokenBlacklistRepo;
            _otpRepo = otpRepo;
            _emailService = emailService;
        }

        public async Task<object> LoginAsync(LoginRequestDto dto)
        {
            var account = await _accountRepo.GetByEmailAsync(dto.Email);

            if (account == null || account.Status != "Active")
                throw new Exception("Invalid credentials");

            var hasher = new PasswordHasher<Models.Account>();

            var result = hasher.VerifyHashedPassword(
                account,
                account.PasswordHash,
                dto.Password
            );

            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Invalid credentials");

            var token = GenerateJwt(account);

            var loginUser = new LoginResponseUserDto
            {
                AccountId = account.AccountId,
                Email = account.Email,
                Name = account.UserProfile.FirstName + " " + account.UserProfile.LastName,
                Role = account.Role.RoleName
            };

            return new LoginResponseDto
            {
                AccessToken = token,
                User = loginUser
            };
        }

        public async Task LogoutAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("Token missing", nameof(token));

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwt;
            try
            {
                jwt = handler.ReadJwtToken(token);
            }
            catch
            {
                throw new ArgumentException("Invalid token", nameof(token));
            }

            var expiry = jwt.ValidTo;
            var blacklisted = new BlackListedToken
            {
                BlacklistedTokenId = Guid.NewGuid(),
                Token = token,
                ExpiresAt = expiry,
            };

            await _tokenBlacklistRepo.AddAsync(blacklisted);
        }

        public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            var existing = await _accountRepo.GetByEmailAsync(dto.Email);
            if(existing != null)    throw new Exception("Email already in use");

            var account = new Models.Account
            {
                AccountId = Guid.NewGuid(),
                RoleId = 2,
                Email = dto.Email,
                Status = "Active",
                PhoneNumber = dto.PhoneNumber ?? string.Empty,
                UserProfile = new UserProfile
                {
                    UserProfileId = Guid.NewGuid(),
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                },
            };

            var hasher = new PasswordHasher<Models.Account>();
            account.PasswordHash = hasher.HashPassword(account, dto.Password);
            var created = await _accountRepo.CreateAsync(account);

            var newCart = new Models.Cart
            {
                CartId = Guid.NewGuid(),
                Customer = created.UserProfile,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _cartRepo.CreateCart(newCart);

            var token = GenerateJwt(created);

            var loginUser = new LoginResponseUserDto
            {
                AccountId = created.AccountId,
                Email = created.Email,
                Name = account.UserProfile.FirstName + " " + account.UserProfile.LastName,
                Role = created.Role.RoleName ?? "Customer"
            };

            return new LoginResponseDto
            {
                AccessToken = token,
                User = loginUser
            };
        }

        private string GenerateJwt(Models.Account account)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
        new Claim(ClaimTypes.Email, account.Email),
        new Claim(ClaimTypes.Role, account.Role.RoleName)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task RequestResetPasswordAsync(string email)
        {
            var user = await _accountRepo.GetByEmailAsync(email);
            if (user == null)
                throw new Exception("Email not found");

            var otpCode = new Random().Next(100000, 999999).ToString();

            var otp = new PasswordResetOtp
            {
                Email = email,
                OtpCode = otpCode,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            await _otpRepo.AddAsync(otp);
            await _otpRepo.SaveChangesAsync();

            await _emailService.SendEmailAsync(email, "Reset Password OTP", $"Your OTP is: {otpCode}");
        }

        public async Task ResetPasswordAsync(ConfirmResetPasswordDto dto)
        {
            var otp = await _otpRepo.GetValidOtpAsync(dto.Email, dto.Otp);

            if (otp == null)
                throw new Exception("Invalid or expired OTP");

            var user = await _accountRepo.GetByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("User not found");

            var hasher = new PasswordHasher<Models.Account>();
            user.PasswordHash = hasher.HashPassword(user, dto.NewPassword);

            otp.IsUsed = true;

            await _accountRepo.UpdateAsync(user);
            await _otpRepo.SaveChangesAsync();
        }

    }
}
