using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories
{
    public class TokenBlacklistRepository : ITokenBlacklistRepository
    {
        private readonly AppDbContext _context;

        public TokenBlacklistRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BlackListedToken token)
        {
            await _context.BlacklistedTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsBlacklistedAsync(string token)
        {
            return await _context.BlacklistedTokens
                .AnyAsync(x => x.Token == token);
        }
    }
}
