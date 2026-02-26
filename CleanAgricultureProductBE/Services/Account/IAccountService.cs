using CleanAgricultureProductBE.DTOs.Account;
using CleanAgricultureProductBE.DTOs.ApiResponse;

namespace CleanAgricultureProductBE.Services.Account
{
    public interface IAccountService
    {
        public Task<ResponseDtoWithPagination<List<AccountResponseDto>>> GetAllAccounts(int? page, int? size, string? keyword);
    }
}
