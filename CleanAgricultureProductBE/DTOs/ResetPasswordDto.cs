using System.ComponentModel.DataAnnotations;

namespace CleanAgricultureProductBE.DTOs
{
    public class ResetPasswordDto
    {
        public string? Oldpassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
