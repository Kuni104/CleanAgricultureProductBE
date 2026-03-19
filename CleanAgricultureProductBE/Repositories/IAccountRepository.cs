using CleanAgricultureProductBE.Enum;
using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories
{
    public interface IAccountRepository
    {
        public Task<List<Account>> GetAllAccountsAsync();
        public Task<List<Account>> GetAllAccountsWithPaginationAsync(int offset, int pageSize, string keyword, AccountRoleEnum accountRole);
        public Task<Account?> GetByIdAsync(Guid accountId);
        public Task<Account?> GetByEmailAsync(string email);
        public Task<Account> CreateAsync(Account account);
        public Task UpdateAsync(Models.Account account);
        public Task SaveChangeAsync();
    }
}
