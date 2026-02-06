using CleanAgricultureProductBE.DTOs;

namespace CleanAgricultureProductBE.Services
{
    public interface IAuthService
    {
        Task<object> LoginAsync(LoginRequestDto dto);
        Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto);
    }
}
