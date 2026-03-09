using System.ComponentModel.DataAnnotations;

namespace CleanAgricultureProductBE.DTOs.DeliveryFee
{
    public class CreateDeliveryFeeRequestDto
    {
        public decimal FromKilometer { get; set; }
        public decimal ToKilometer { get; set;}
        public decimal FeeAmount { get; set; }
        
    }
}
