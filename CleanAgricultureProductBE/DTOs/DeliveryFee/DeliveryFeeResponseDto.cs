namespace CleanAgricultureProductBE.DTOs.DeliveryFee
{
    public class DeliveryFeeResponseDto
    {
        public Guid DeliveryFeeId { get; set;}
        public string City { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public decimal FeeAmount { get; set; }

    }
}
