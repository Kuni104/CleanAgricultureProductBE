namespace CleanAgricultureProductBE.DTOs.DeliveryFee
{
    public class DeliveryFeeResponseDto
    {
        public Guid DeliveryFeeId { get; set;}
        public decimal FromKilometer { get; set;}
        public decimal ToKilometer { get; set;}
        public decimal FeeAmount { get; set;}

    }
}
