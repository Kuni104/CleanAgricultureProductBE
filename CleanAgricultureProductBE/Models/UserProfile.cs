namespace CleanAgricultureProductBE.Models
{
    public class UserProfile
    {
        public Guid UserProfileId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public Account Account { get; set; } = null!;
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
