namespace CleanAgricultureProductBE.Models
{
    public class OrderDetail
    {
        public Guid OrderDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiryDate { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
