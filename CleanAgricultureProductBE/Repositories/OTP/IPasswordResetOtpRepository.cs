using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories.OTP
{
    public interface IPasswordResetOtpRepository
    {
        Task AddAsync(PasswordResetOtp otp);
        Task<PasswordResetOtp> GetValidOtpAsync(string email, string otp);
        Task SaveChangesAsync();
    }
}
