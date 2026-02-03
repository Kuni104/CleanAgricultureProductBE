namespace CleanAgricultureProductBE.Models
{
    public class Account
    {
        public Guid AccountId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int RoleId { get; set; }

        public UserProfile UserProfile { get; set; } = null!;
        public Role Role { get; set; } = null!;
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}
