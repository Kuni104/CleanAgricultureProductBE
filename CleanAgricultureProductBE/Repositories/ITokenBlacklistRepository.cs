using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories
{
    public interface ITokenBlacklistRepository
    {
        Task AddAsync(BlackListedToken token);
        Task<bool> IsBlacklistedAsync(string token);
    }
}
