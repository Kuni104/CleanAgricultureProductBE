namespace CleanAgricultureProductBE.Models
{
    public class EmailOtp
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public DateTime ExpiredAt { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
