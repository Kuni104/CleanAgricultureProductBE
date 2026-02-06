using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _context;

        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Account> CreateAsync(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            
            var created = await _context.Accounts
                                .Include(a => a.UserProfile)
                                .Include(a => a.Role)
                                .FirstOrDefaultAsync(a => a.AccountId == account.AccountId);

            return created ?? account;
        }

        public Task<Account?> GetByEmailAsync(string email)
        {
            return _context.Accounts.Include(a => a.UserProfile)
                                    .Include(a => a.Role)
                                    .FirstOrDefaultAsync(a => a.Email == email);
        }
    }
}
