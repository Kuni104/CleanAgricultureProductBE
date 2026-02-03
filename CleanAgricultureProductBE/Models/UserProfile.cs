namespace CleanAgricultureProductBE.Models
{
    public class UserProfile
    {
        public Guid UserProfileId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public Account Account { get; set; } = null!;

    }
}
