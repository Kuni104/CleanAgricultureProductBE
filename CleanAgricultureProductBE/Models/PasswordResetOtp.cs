namespace CleanAgricultureProductBE.Models
{
    public class PasswordResetOtp
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string OtpCode { get; set; }
        public DateTime ExpiredAt { get; set; }
        public bool IsUsed { get; set; }
    }
}
