namespace CleanAgricultureProductBE.DTOs.OrderDetail
{
    public class OrderDetailListResponseDto
    {
        public Guid OrderId { get; set; }
        public List<OrderDetailResponseDto> OrderDetails { get; set; } = new List<OrderDetailResponseDto>();
        public string Address { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
    }
}
