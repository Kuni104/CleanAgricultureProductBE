using CleanAgricultureProductBE.DTOs;

namespace CleanAgricultureProductBE.Services
{
    public interface IAuthService
    {
        Task<object> LoginAsync(LoginRequestDto dto);
        Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto);
        Task LogoutAsync(string token);
        Task RequestResetPasswordAsync(string email);
        Task ResetPasswordAsync(ConfirmResetPasswordDto dto);
    }
}
