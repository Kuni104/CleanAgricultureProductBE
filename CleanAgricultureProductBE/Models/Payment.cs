namespace CleanAgricultureProductBE.Models
{
    public class Payment
    {
        public Guid PaymentId { get; set; }
        public int PaymentMethodId { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string? TransactionCode { get; set; }
        public DateTime CreatedAt { get; set; }
        /*------------------------------------------------------------------------------------------------------------------------*/
        public PaymentMethod PaymentMethod { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = new List<Order>();

    }
}
