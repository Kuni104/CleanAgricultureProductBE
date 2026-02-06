using CleanAgricultureProductBE.DTOs.DeliveryFee;
using CleanAgricultureProductBE.Repositories.DeliveryFee;

namespace CleanAgricultureProductBE.Services.DeliveryFee
{
    public class DeliveryFeeService(IDeliveryFeeRepository deliveryFeeRepository) : IDeliveryFeeService
    {
        public async Task<List<GetDeliveryFeeResponseDto>> GetDeliveryFeeList()
        {
            var deliveryFees = await deliveryFeeRepository.GetDeliveryFeeList();

            var deliveryFeeList = deliveryFees.Select(df => new GetDeliveryFeeResponseDto
            {
                DeliveryFeeId = df.DeliveryFeeId,
                District = df.District,
                City = df.City,
                FeeAmount = df.FeeAmount,
                EstimatedDay = df.EstimatedDay,
                EffectiveDay = df.EffectiveDay
            }).ToList();

            return deliveryFeeList;
        }

        public async Task<GetDeliveryFeeResponseDto> AddDeliveryFee(CreateDeliveryFeeRequestDto request)
        {
            var newDeliveryFee = new Models.DeliveryFee
            {
                DeliveryFeeId = Guid.NewGuid(),
                District = request.District,
                City = request.City,
                FeeAmount = request.FeeAmount,
                EstimatedDay = request.EstimatedDay,
                EffectiveDay = request.EffectiveDay
            };

            await deliveryFeeRepository.AddDeliveryFee(newDeliveryFee);

            var deliveryFeeDto = new GetDeliveryFeeResponseDto
            {
                DeliveryFeeId = newDeliveryFee.DeliveryFeeId,
                District = newDeliveryFee.District,
                City = newDeliveryFee.City,
                FeeAmount = newDeliveryFee.FeeAmount,
                EstimatedDay = newDeliveryFee.EstimatedDay,
                EffectiveDay = newDeliveryFee.EffectiveDay
            };

            return deliveryFeeDto;
        }
    }
}
