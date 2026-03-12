using CleanAgricultureProductBE.DTOs.Account;
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Order;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Repositories.Order;
using Microsoft.AspNetCore.Identity;

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
                if (page <= 0)
                    throw new ArgumentException("Số trang (page) phải lớn hơn 0");
                if (size <= 0)
                    throw new ArgumentException("Kích thước trang (size) phải lớn hơn 0");

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

        public async Task<AccountResponseDto> CreateAccount(CreateAccountRequestDto request)
        {
            var existAccount = await accountRepository.GetByEmailAsync(request.Email);
            if (existAccount != null)
            {
                return null!;
            }

            var account = new Models.Account
            {
                AccountId = Guid.NewGuid(),
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RoleId = request.RoleId,
                Status = "Active",
                UserProfile = new Models.UserProfile
                {
                    UserProfileId = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName
                }
                
            };

            var hasher = new PasswordHasher<Models.Account>();
            account.PasswordHash = hasher.HashPassword(account, request.Password);
            var created = await accountRepository.CreateAsync(account);

            return new AccountResponseDto
            {
                AccountId = account.AccountId,
                Email = request.Email,
                Name = account.UserProfile.FirstName + " " + account.UserProfile.LastName,
                Role = account.Role.RoleName,
                Status = account.Status,
            };
        }

        public async Task<AccountResponseDto> ChangeAccountStatus(Guid accountId, ChangeAccountStatusRequestDto request)
        {
            var account = await accountRepository.GetByIdAsync(accountId);
            if (account == null) {
                return null!;
            }


            var accountStatus = char.ToUpper(request.Status[0]) + request.Status.Substring(1);
            account.Status = accountStatus;
            await accountRepository.UpdateAsync(account);

            return new AccountResponseDto
            {
                AccountId = account.AccountId,
                Email = account.Email,
                Name = account.UserProfile.FirstName + " " + account.UserProfile.LastName,
                Role = account.Role.RoleName,
                Status = account.Status,
            };
        }
    }
}
