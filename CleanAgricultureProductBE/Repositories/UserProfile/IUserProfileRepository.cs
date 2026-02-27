namespace CleanAgricultureProductBE.Repositories.UserProfile
{
    public interface IUserProfileRepository
    {
        public Task GetUserProfile(string email);
        public Task UpdateUserProfile(Models.UserProfile userProfile);
    }
}
