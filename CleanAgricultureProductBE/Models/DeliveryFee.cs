namespace CleanAgricultureProductBE.Models
{
    public class DeliveryFee
    {
        public Guid DeliveryFeeId { get; set; }
        public string City { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public decimal FeeAmount { get; set; }
        /*------------------------------------------------------------------------------------------------------------------------*/
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
