namespace CleanAgricultureProductBE.DTOs.Order
{
    public class PlaceOrderResponseDto
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime? Schedule { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentUrl { get; set; } = string.Empty;
    }
}
