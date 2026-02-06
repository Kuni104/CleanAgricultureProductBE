namespace CleanAgricultureProductBE.Models
{
    public class BlackListedToken
    {
        public Guid BlacklistedTokenId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
