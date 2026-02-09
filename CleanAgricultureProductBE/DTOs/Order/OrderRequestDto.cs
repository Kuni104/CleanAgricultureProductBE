using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.DTOs.Order
{
    public class OrderRequestDto
    {
        public Guid AddressId { get; set; }
        public Guid? ScheduleId { get; set; }
        public Guid DeliveryFeeId { get; set; }
        public int PaymentMethodId { get; set; }
    }
}
