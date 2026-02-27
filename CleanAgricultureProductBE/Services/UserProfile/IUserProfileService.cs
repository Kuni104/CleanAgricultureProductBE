using CleanAgricultureProductBE.DTOs.UserProfile;

namespace CleanAgricultureProductBE.Services.UserProfile
{
    public interface IUserProfileService
    {
        public Task<UserProfileResponseDto> GetUserProfile(string email);
        public Task<UserProfileResponseDto> UpdateUserProfile(string email, UserProfileRequestDto request);
    }
}
