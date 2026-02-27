using CleanAgricultureProductBE.DTOs.UserProfile;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Repositories.Cart;
using CleanAgricultureProductBE.Repositories.UserProfile;

namespace CleanAgricultureProductBE.Services.UserProfile
{
    public class UserProfileService(IAccountRepository accountRepository, IUserProfileRepository userProfileRepository) : IUserProfileService
    {
        public async Task<UserProfileResponseDto> GetUserProfile(string email)
        {
            var account = await accountRepository.GetByEmailAsync(email);
            var customer = account!.UserProfile;

            return new UserProfileResponseDto
            {
               FirstName = customer.FirstName,
               LastName = customer.LastName,
               Email = account.Email,
               PhoneNumber = account.PhoneNumber
            };
        }

        public async Task<UserProfileResponseDto> UpdateUserProfile(string email, UserProfileRequestDto request)
        {
            var account = await accountRepository.GetByEmailAsync(email);
            var customer = account!.UserProfile;

            account.PhoneNumber = request.PhoneNumber.Trim() == "" ? account.PhoneNumber : request.PhoneNumber;
            customer.FirstName = request.FirstName.Trim() == "" ? customer.FirstName : request.FirstName;
            customer.LastName = request.LastName.Trim() == "" ? customer.LastName : request.LastName;

            await accountRepository.UpdateAsync(account);
            await userProfileRepository.UpdateUserProfile(customer);  

            return new UserProfileResponseDto
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = account.Email,
                PhoneNumber = account.PhoneNumber
            };
        }
    }
}
