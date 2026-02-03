namespace CleanAgricultureProductBE.Models
{
    public class Order
    {
        public Guid Id { get; set; }

        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public Guid AddressId { get; set; }
        public Guid ScheduleId { get; set; }
        public Guid DeliveryFeeId { get; set; }
        public Guid PaymentId { get; set; }


        public UserProfile Customer { get; set; } = null!;
        public Address Address { get; set; } = null!;
        public Schedule Schedule { get; set; } = null!;
        public DeliveryFee DeliveryFee { get; set; } = null!;
        public Payment Payment { get; set; } = null!;

    }
}
