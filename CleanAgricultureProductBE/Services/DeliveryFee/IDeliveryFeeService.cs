using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.DeliveryFee;

namespace CleanAgricultureProductBE.Services.DeliveryFee
{
    public interface IDeliveryFeeService
    {
        public Task<List<DeliveryFeeResponseDto>> GetDeliveryFeeList();
        public Task<DeliveryFeeResponseDto> GetDeliveryFeeById(Guid deliveryFeeId);
        public Task<ResultStatusWithData<GetDeliveryFeeByAddressResponseDto>> GetDeliveryFeeByAddress(Guid addressId);
        public Task<DeliveryFeeResponseDto> AddDeliveryFee(CreateDeliveryFeeRequestDto request);
        public Task<ResultStatusWithData<DeliveryFeeResponseDto>> UpdateDeliveryFee(Guid deliveryFeeId, UpdateDeliveryFeeRequestDto request);
        public Task<bool> DeleteDeliveryFeeById(Guid deliveryFeeId);
    }
}
