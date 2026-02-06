namespace CleanAgricultureProductBE.DTOs.DeliveryFee
{
    public class CreateDeliveryFeeRequestDto
    {
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal FeeAmount { get; set; }
        public DateTime EstimatedDay { get; set; }
        public DateTime EffectiveDay { get; set; }
    }
}
