namespace CleanAgricultureProductBE.DTOs
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public LoginResponseUserDto? User { get; set; }
    }
}
