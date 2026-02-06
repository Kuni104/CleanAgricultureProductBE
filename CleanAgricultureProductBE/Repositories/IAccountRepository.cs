using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetByEmailAsync(string email);
        Task<Account> CreateAsync(Account account); 
    }
}
