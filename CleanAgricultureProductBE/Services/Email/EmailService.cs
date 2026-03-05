using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories.OTP;
using CleanAgricultureProductBE.Services.OTP;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace CleanAgricultureProductBE.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IEmailOtpRepository _otpRepository;
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IEmailOtpRepository otpRepository, IOptions<SmtpSettings> smtpOptions)
        {
            _otpRepository = otpRepository;
            _smtpSettings = smtpOptions.Value;
        }

        public async Task SendEmailAsync(string toEmail, string otp)
        {
            using var mail = new MailMessage();
            mail.From = new MailAddress(_smtpSettings.Email);
            mail.To.Add(toEmail);
            mail.Subject = "Your OTP Code";
            mail.Body = $"Your OTP is {otp}. It is valid for 5 minutes.";

            using var smtp = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port);
            smtp.Credentials = new NetworkCredential(
                _smtpSettings.Email,
                _smtpSettings.Password
            );
            smtp.EnableSsl = true;

            await smtp.SendMailAsync(mail);
        }
    }
}
