
using CleanAgricultureProductBE.Data;

namespace CleanAgricultureProductBE.Repositories.UserProfile
{
    public class UserProfileRepository(AppDbContext context) : IUserProfileRepository
    {
        public Task GetUserProfile(string email)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateUserProfile(Models.UserProfile userProfile)
        {
            context.UserProfiles.Update(userProfile);
            await context.SaveChangesAsync();
        }
    }
}
