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

        public AuthService(IAccountRepository accountRepo, IConfiguration config)
        {
            _accountRepo = accountRepo;
            _config = config;
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

            return new
            {
                token = token,
                accountId = account.AccountId,
                email = account.Email,
                role = account.Role.RoleName
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
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
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
