namespace CleanAgricultureProductBE.Models
{
    public class Cart
    {
        public Guid CartId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid CustomerId { get; set; }

        public UserProfile Customer { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
