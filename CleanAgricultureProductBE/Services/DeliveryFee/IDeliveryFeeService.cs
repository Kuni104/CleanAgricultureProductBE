using CleanAgricultureProductBE.DTOs.DeliveryFee;

namespace CleanAgricultureProductBE.Services.DeliveryFee
{
    public interface IDeliveryFeeService
    {
        public Task<List<GetDeliveryFeeResponseDto>> GetDeliveryFeeList();
        public Task<GetDeliveryFeeResponseDto> AddDeliveryFee(CreateDeliveryFeeRequestDto request);
    }
}
