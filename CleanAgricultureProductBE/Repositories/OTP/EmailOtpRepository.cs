using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.OTP
{
    public class EmailOtpRepository : IEmailOtpRepository
    {
        private readonly AppDbContext _context;
        public EmailOtpRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(EmailOtp otp)
        {
            await _context.EmailOtps.AddAsync(otp);
        }

        public async Task<EmailOtp?> GetValidOtpAsync(string email, string otpCode)
        {
            return await _context.EmailOtps
                .Where(x => x.Email == email
                         && x.OtpCode == otpCode
                         && x.IsUsed == false)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
