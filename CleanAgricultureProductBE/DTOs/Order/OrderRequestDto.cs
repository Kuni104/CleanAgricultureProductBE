using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.DTOs.Order
{
    public class OrderRequestDto
    {
        public Guid AddressId { get; set; }
        public Guid DeliveryFeeId { get; set; }
        public int PaymentMethodId { get; set; }
        public bool IsCycleSchedule { get; set; } = false;
        public int? DayCycle { get; set; }
        public bool IsMonthly { get; set; } = false;
    }
}
