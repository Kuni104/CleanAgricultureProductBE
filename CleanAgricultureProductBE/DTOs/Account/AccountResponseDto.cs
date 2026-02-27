namespace CleanAgricultureProductBE.DTOs.Account
{
    public class AccountResponseDto
    {
        public Guid AccountId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Status {  get; set; } = string.Empty;
    }
}
