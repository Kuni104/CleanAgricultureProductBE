namespace CleanAgricultureProductBE.Models
{
    public class CartItem
    {
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }

        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
