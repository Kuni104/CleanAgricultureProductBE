using CleanAgricultureProductBE.DTOs.Account;
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.Enum;

namespace CleanAgricultureProductBE.Services.Account
{
    public interface IAccountService
    {
        public Task<ResponseDtoWithPagination<List<AccountResponseDto>>> GetAllAccounts(int? page, int? size, string? keyword, AccountRoleEnum accountRole);
        public Task<AccountResponseDto> CreateAccount(CreateAccountRequestDto request);
        public Task<AccountResponseDto> ChangeAccountStatus(Guid accountId, ChangeAccountStatusRequestDto request);
    }
}
