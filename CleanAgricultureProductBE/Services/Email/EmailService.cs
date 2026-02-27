using System.Net;
using System.Net.Mail;

namespace CleanAgricultureProductBE.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(
                    _config["Smtp:Email"],
                    _config["Smtp:Password"]
                ),
                EnableSsl = true
            };

            var mail = new MailMessage(
                _config["Smtp:Email"],
                to,
                subject,
                body
            );

            await smtp.SendMailAsync(mail);
        }
    }
}
