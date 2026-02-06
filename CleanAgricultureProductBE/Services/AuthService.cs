using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories;
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
        private readonly IConfiguration _config;
        private readonly ITokenBlacklistRepository _tokenBlacklistRepo;

        public AuthService(IAccountRepository accountRepo, IConfiguration config, ITokenBlacklistRepository tokenBlacklistRepo)
        {
            _accountRepo = accountRepo;
            _config = config;
            _tokenBlacklistRepo = tokenBlacklistRepo;
        }

        public async Task<object> LoginAsync(LoginRequestDto dto)
        {
            var account = await _accountRepo.GetByEmailAsync(dto.Email);

            if (account == null || account.Status != "Active")
                throw new Exception("Invalid credentials");

            var hasher = new PasswordHasher<Account>();

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
                Token = token,
                AccountId = account.AccountId,
                Email = account.Email,
                Role = account.Role.RoleName
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

            var account = new Account
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
                }
            };

            var hasher = new PasswordHasher<Account>();
            account.PasswordHash = hasher.HashPassword(account, dto.Password);
            var created = await _accountRepo.CreateAsync(account);
            var token = GenerateJwt(created);
            return new LoginResponseDto
            {
                Token = token,
                AccountId = created.AccountId,
                Email = created.Email,
                Role = created.Role.RoleName ?? "Customer"
            };
        }

        private string GenerateJwt(Account account)
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
                expires: DateTime.Now.AddHours(2),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
