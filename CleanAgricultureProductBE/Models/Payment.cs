namespace CleanAgricultureProductBE.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int PaymentMethodId { get; set; }

        public PaymentMethod PaymentMethod { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
