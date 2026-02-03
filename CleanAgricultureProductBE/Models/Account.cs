namespace CleanAgricultureProductBE.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int RoleId { get; set; }

        public Role Role { get; set; } = null!;
    }
}
