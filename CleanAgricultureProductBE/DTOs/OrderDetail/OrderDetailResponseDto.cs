namespace CleanAgricultureProductBE.DTOs.OrderDetail
{
    public class OrderDetailResponseDto
    {
        public Guid OrderDetailId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
