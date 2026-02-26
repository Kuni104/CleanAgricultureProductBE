namespace CleanAgricultureProductBE.DTOs.Account
{
    public class CreateAccountRequestDto
    {
        public string Email {  get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int RoleId { get; set; }

    }
}
