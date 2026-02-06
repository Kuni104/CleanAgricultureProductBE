using CleanAgricultureProductBE.DTOs.DeliveryFee;

namespace CleanAgricultureProductBE.Services.DeliveryFee
{
    public interface IDeliveryFeeService
    {
        public Task<List<GetDeliveryFeeResponseDto>> GetDeliveryFeeList();
        public Task<GetDeliveryFeeResponseDto> AddDeliveryFee(CreateDeliveryFeeRequestDto request);
        public Task<GetDeliveryFeeResponseDto> UpdateDeliveryFee(GetDeliveryFeeResponseDto request);
        public Task<bool> DeleteDeliveryFeeById(Guid deliveryFeeId);
    }
}
