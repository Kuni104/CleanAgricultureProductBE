namespace CleanAgricultureProductBE.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        /*------------------------------------------------------------------------------------------------------------------------*/
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
