namespace CleanAgricultureProductBE.DTOs
{
    public class FotgotPasswordDto
    {
        public string? Email { get; set; }
        public string? OtpCode { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
