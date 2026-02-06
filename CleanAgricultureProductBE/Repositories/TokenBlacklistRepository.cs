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
            _context.Set<BlackListedToken>().Add(token);
            await _context.SaveChangesAsync();
        }

        public Task<bool> IsBlacklistedAsync(string token)
        {
            return _context.Set<BlackListedToken>()
                .AnyAsync(t => t.Token == token);
        }
    }
}
