namespace CleanAgricultureProductBE.Services.OTP
{
    public interface IEmailOtpService
    {
        Task SendOtpAsync(string email);
        Task<bool> VerifyOtpAsync(string email, string otpCode);
    }
}
