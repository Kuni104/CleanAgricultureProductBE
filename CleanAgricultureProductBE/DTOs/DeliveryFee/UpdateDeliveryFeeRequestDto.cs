using System.ComponentModel.DataAnnotations;

namespace CleanAgricultureProductBE.DTOs.DeliveryFee
{
    public class UpdateDeliveryFeeRequestDto
    {
        public string? District { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? FeeAmount { get; set; }
        public DateTime? EstimatedDay { get; set; }
        public DateTime? EffectiveDay { get; set; }
    }
}
