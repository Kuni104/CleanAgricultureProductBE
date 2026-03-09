using CleanAgricultureProductBE.DTOs;
using Microsoft.AspNetCore.Identity.Data;

namespace CleanAgricultureProductBE.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto);
        Task LogoutAsync(string token);
        Task RequestResetPasswordAsync(string email);
        Task ForgotPasswordAsync(FotgotPasswordDto dto);
        Task ChangePasswordAsync(Guid accountId, ResetPasswordDto dto);
    }
}
