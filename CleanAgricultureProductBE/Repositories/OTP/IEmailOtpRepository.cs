using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories.OTP
{
    public interface IEmailOtpRepository
    {
        Task AddAsync(EmailOtp otp);
        Task<EmailOtp?> GetValidOtpAsync(string email, string otpCode);
        Task SaveChangesAsync();
    }
}
