namespace CleanAgricultureProductBE.Models
{
    public class DeliveryFee
    {
        public Guid DeliveryFeeId { get; set; }
        public decimal FromKilometer { get; set; }
        public decimal ToKilometer { get; set; }
        public decimal FeeAmount { get; set; }
        /*------------------------------------------------------------------------------------------------------------------------*/
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
