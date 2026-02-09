namespace CleanAgricultureProductBE.DTOs.Order
{
    public class OrderResponseDto
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid AddressId { get; set; }
        public Guid? ScheduleId { get; set; }
        public Guid PaymentId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus {  get; set; } = string.Empty;
    }
}
