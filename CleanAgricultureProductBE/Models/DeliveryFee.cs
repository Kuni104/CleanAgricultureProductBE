namespace CleanAgricultureProductBE.Models
{
    public class DeliveryFee
    {
        public Guid DeliveryFeeId { get; set; }
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal FeeAmount { get; set; }
        public DateTime EstimatedDay { get; set; }
        public DateTime EffectiveDay { get; set; }
        /*------------------------------------------------------------------------------------------------------------------------*/
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
