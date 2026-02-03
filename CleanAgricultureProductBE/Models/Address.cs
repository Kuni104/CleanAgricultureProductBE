namespace CleanAgricultureProductBE.Models
{
    public class Address
    {
        public Guid AddressId { get; set; }
        public Guid UserProfileId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string RecipientPhone { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string AddressDetail { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        /*------------------------------------------------------------------------------------------------------------------------*/
        public UserProfile UserProfile { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
