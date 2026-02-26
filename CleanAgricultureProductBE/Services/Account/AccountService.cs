using CleanAgricultureProductBE.DTOs.Account;
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Order;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Repositories.Order;

namespace CleanAgricultureProductBE.Services.Account
{
    public class AccountService(IAccountRepository accountRepository) : IAccountService
    {
        public async Task<ResponseDtoWithPagination<List<AccountResponseDto>>> GetAllAccounts(int? page, int? size, string? keyword)
        {
            bool isPagination = false;
            int offset = 0;
            int pageSize = 0;
            if (page != null && size != null)
            {
                pageSize = (int)size;
                offset = (int)((page - 1) * size);
                isPagination = true;
            }

            var accounts = await accountRepository.GetAllAccountsAsync();

            int totalItems = accounts.Count;

            if (isPagination)
            {
                accounts = await accountRepository.GetAllAccountsWithPaginationAsync(offset, pageSize);
            }

            var accountResponseList = new List<AccountResponseDto>();
            foreach (var ac in accounts)
            {
                accountResponseList.Add(new AccountResponseDto
                {
                    AccountId = ac.AccountId,
                    Email = ac.Email,
                    Name = ac.UserProfile.FirstName + " " + ac.UserProfile.LastName,
                    Role = ac.Role.RoleName,
                    Status = ac.Status
                });
            }

            var result = new ResponseDtoWithPagination<List<AccountResponseDto>>
            {
                ResultObject = accountResponseList
            };

            if (isPagination)
            {
                int totalPage = totalItems / pageSize + (totalItems % pageSize == 0 ? 1 : 0);
                if (totalItems < pageSize || page <= size)
                {
                    totalPage = 1;
                }

                result.Pagination = new Pagination
                {
                    PageNumber = (int)page!,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPage
                };
            }

            return result;
        }
    }
}
