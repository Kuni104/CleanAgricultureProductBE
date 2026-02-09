namespace CleanAgricultureProductBE.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid AddressId { get; set; }
        public Guid? ScheduleId { get; set; }
        public Guid DeliveryFeeId { get; set; }
        public Guid? PaymentId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        /*------------------------------------------------------------------------------------------------------------------------*/
        public UserProfile Customer { get; set; } = null!;
        public Address Address { get; set; } = null!;
        public Schedule Schedule { get; set; } = null!;
        public DeliveryFee DeliveryFee { get; set; } = null!;
        public Payment Payment { get; set; } = null!;
        public Complaint Complaint { get; set; } = null!;
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public ICollection<CycleSchedule> CycleSchedules { get; set; } = new List<CycleSchedule>();
    }
}
