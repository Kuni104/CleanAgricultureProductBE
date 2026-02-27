namespace CleanAgricultureProductBE.DTOs.OrderDetail
{
    public class OrderDetailListResponseDto
    {
        public Guid OrderId { get; set; }
        public List<OrderDetailResponseDto> OrderDetails { get; set; } = new List<OrderDetailResponseDto>();
        public decimal TotalPrice { get; set; }
    }
}
