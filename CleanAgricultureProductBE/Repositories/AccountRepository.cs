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

        public Task<Account?> GetByEmailAsync(string email)
        {
            return _context.Accounts.Include(a => a.UserProfile)
                                    .Include(a => a.Role)
                                    .FirstOrDefaultAsync(a => a.Email == email);
        }

        public Task<Account?> GetAccountById(string accountId)
        {
            return _context.Accounts.Include(a => a.UserProfile)
                                    .Include(a => a.Role)
                                    .FirstOrDefaultAsync(a => a.AccountId.ToString() == accountId);
        }
    }
}
