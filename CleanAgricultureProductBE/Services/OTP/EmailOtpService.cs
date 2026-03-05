using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories.OTP;
using System.Net;
using System.Net.Mail;

namespace CleanAgricultureProductBE.Services.OTP
{
    public class EmailOtpService : IEmailOtpService
    {
        private readonly IEmailOtpRepository _otpRepository;

        public EmailOtpService(IEmailOtpRepository otpRepository)
        {
            _otpRepository = otpRepository;
        }

        public async Task SendOtpAsync(string email)
        {
            var otpCode = new Random().Next(100000, 999999).ToString();

            var otp = new EmailOtp
            {
                Email = email,
                OtpCode = otpCode,
                CreatedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddMinutes(5),
                IsUsed = false
            };

            await _otpRepository.AddAsync(otp);
            await _otpRepository.SaveChangesAsync();

            await SendEmail(email, otpCode);
        }

        public async Task<bool> VerifyOtpAsync(string email, string otpCode)
        {
            var record = await _otpRepository.GetValidOtpAsync(email, otpCode);

            if (record == null)
                return false;

            if (record.ExpiredAt < DateTime.UtcNow)
                return false;

            record.IsUsed = true;
            await _otpRepository.SaveChangesAsync();

            return true;
        }

        private async Task SendEmail(string toEmail, string otp)
        {
            string senderEmail = "trungvnse182447@fpt.edu.vn";
            string appPassword = "gksozymnhlifkmuw";

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(senderEmail);
            mail.To.Add(toEmail);
            mail.Subject = "Your OTP Code";
            mail.Body = $"Your OTP is {otp}. It is valid for 5 minutes.";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(senderEmail, appPassword);
            smtp.EnableSsl = true;

            await smtp.SendMailAsync(mail);
        }
    }
}
