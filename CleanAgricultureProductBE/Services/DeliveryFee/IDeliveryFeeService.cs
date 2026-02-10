using CleanAgricultureProductBE.DTOs.DeliveryFee;

namespace CleanAgricultureProductBE.Services.DeliveryFee
{
    public interface IDeliveryFeeService
    {
        public Task<List<DeliveryFeeResponseDto>> GetDeliveryFeeList();
        public Task<DeliveryFeeResponseDto> GetDeliveryFeeById(Guid deliveryFeeId);
        public Task<DeliveryFeeResponseDto> AddDeliveryFee(CreateDeliveryFeeRequestDto request);
        public Task<DeliveryFeeResponseDto> UpdateDeliveryFee(Guid deliveryFeeId, UpdateDeliveryFeeRequestDto request);
        public Task<bool> DeleteDeliveryFeeById(Guid deliveryFeeId);
    }
}
