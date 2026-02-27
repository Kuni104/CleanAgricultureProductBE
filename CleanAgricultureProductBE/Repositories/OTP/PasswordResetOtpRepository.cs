using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.OTP
{
    public class PasswordResetOtpRepository : IPasswordResetOtpRepository
    {
        private readonly AppDbContext _context;
        public PasswordResetOtpRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PasswordResetOtp otp)
        {
            await _context.PasswordResetOtps.AddAsync(otp);
        }

        public async Task<PasswordResetOtp> GetValidOtpAsync(string email, string otp)
        {
            return await _context.PasswordResetOtps
                .FirstOrDefaultAsync(x =>
                    x.Email == email &&
                    x.OtpCode == otp &&
                    x.ExpiredAt > DateTime.UtcNow &&
                    !x.IsUsed);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
