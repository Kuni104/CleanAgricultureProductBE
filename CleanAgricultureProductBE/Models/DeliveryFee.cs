namespace CleanAgricultureProductBE.Models
{
    public class DeliveryFee
    {
        public Guid Id { get; set; }
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal FeeAmount { get; set; }
        public string EstimatedDay { get; set; } = string.Empty;
        public string EffectiveDay { get; set; } = string.Empty;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
