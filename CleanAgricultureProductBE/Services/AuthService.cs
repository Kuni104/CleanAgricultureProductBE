using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Repositories.Cart;
using CleanAgricultureProductBE.Repositories.OTP;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly IEmailOtpRepository _emailOtpRepo;

        public AuthService(
            IAccountRepository accountRepo,
            ICartRepository cartRepo,
            IConfiguration config,
            ITokenBlacklistRepository tokenBlacklistRepo,
            IEmailOtpRepository emailOtpRepo)
        {
            _accountRepo = accountRepo;
            _cartRepo = cartRepo;
            _config = config;
            _tokenBlacklistRepo = tokenBlacklistRepo;
            _emailOtpRepo = emailOtpRepo;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
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

            return new LoginResponseDto
            {
                AccessToken = token,
                User = new LoginResponseUserDto
                {
                    AccountId = account.AccountId,
                    Email = account.Email,
                    Name = account.UserProfile.FirstName + " " + account.UserProfile.LastName,
                    Role = account.Role.RoleName
                }
            };
        }

        public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            var validOtp = await _emailOtpRepo.GetValidOtpAsync(dto.Email, dto.Otp);

            if (validOtp == null)
                throw new Exception("Invalid or expired OTP");

            validOtp.IsUsed = true;
            await _emailOtpRepo.SaveChangesAsync();

            var existing = await _accountRepo.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new Exception("Email already in use");

            var account = new Models.Account
            {
                AccountId = Guid.NewGuid(),
                RoleId = 2,
                Email = dto.Email,
                Status = "Active",
                PhoneNumber = dto.PhoneNumber ?? string.Empty,
                UserProfile = new Models.UserProfile
                {
                    UserProfileId = Guid.NewGuid(),
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                }
            };

            var hasher = new PasswordHasher<Models.Account>();
            account.PasswordHash = hasher.HashPassword(account, dto.Password);

            var created = await _accountRepo.CreateAsync(account);

            var newCart = new Models.Cart
            {
                CartId = Guid.NewGuid(),
                Customer = created.UserProfile,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cartRepo.CreateCart(newCart);

            var token = GenerateJwt(created);

            return new LoginResponseDto
            {
                AccessToken = token,
                User = new LoginResponseUserDto
                {
                    AccountId = created.AccountId,
                    Email = created.Email,
                    Name = created.UserProfile.FirstName + " " + created.UserProfile.LastName,
                    Role = created.Role.RoleName ?? "Customer"
                }
            };
        }

        public async Task LogoutAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token missing");

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var blacklisted = new BlackListedToken
            {
                BlacklistedTokenId = Guid.NewGuid(),
                Token = token,
                ExpiresAt = jwt.ValidTo
            };

            await _tokenBlacklistRepo.AddAsync(blacklisted);
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

            var otp = new EmailOtp
            {
                Email = email,
                OtpCode = otpCode,
                CreatedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            await _emailOtpRepo.AddAsync(otp);
            await _emailOtpRepo.SaveChangesAsync();
        }

        public async Task ResetPasswordAsync(ConfirmResetPasswordDto dto)
        {
            var otp = await _emailOtpRepo.GetValidOtpAsync(dto.Email, dto.Otp);

            if (otp == null)
                throw new Exception("Invalid or expired OTP");

            var user = await _accountRepo.GetByEmailAsync(dto.Email);
            if (user == null)
                throw new Exception("User not found");

            var hasher = new PasswordHasher<Models.Account>();
            user.PasswordHash = hasher.HashPassword(user, dto.NewPassword);

            otp.IsUsed = true;

            await _accountRepo.UpdateAsync(user);
            await _emailOtpRepo.SaveChangesAsync();
        }

        public async Task ForgotPasswordAsync(FotgotPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                throw new Exception("Mật khẩu xác nhận không trùng khớp");

            var user = await _accountRepo.GetByEmailAsync(dto.Email);

            if (user == null)
                throw new Exception("Email không tồn tại");

            var otpRecord = await _emailOtpRepo.GetValidOtpAsync(dto.Email, dto.OtpCode);

            if (otpRecord == null)
                throw new Exception("OTP không hợp lệ hoặc đã hết hạn");

            var hasher = new PasswordHasher<Models.Account>();
            user.PasswordHash = hasher.HashPassword(user, dto.NewPassword);

            otpRecord.IsUsed = true;

            await _accountRepo.SaveChangeAsync();
        }

        public async Task ChangePasswordAsync(Guid accountId, ResetPasswordDto dto)
        {
            var user = await _accountRepo.GetByIdAsync(accountId);

            if (user == null)
                throw new Exception("User not found");

            var hasher = new PasswordHasher<Models.Account>();

            var result = hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.Oldpassword
            );

            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Mật khẩu cũ không đúng");

            if (dto.NewPassword != dto.ConfirmPassword)
                throw new Exception("Mật khẩu xác nhận không trùng khớp");

            user.PasswordHash = hasher.HashPassword(user, dto.NewPassword);

            await _accountRepo.SaveChangeAsync();
        }
    }
}
