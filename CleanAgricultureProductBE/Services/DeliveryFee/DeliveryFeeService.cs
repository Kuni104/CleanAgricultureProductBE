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

        public async Task<GetDeliveryFeeResponseDto> UpdateDeliveryFee(GetDeliveryFeeResponseDto request)
        {
            var existingDeliveryFee = await deliveryFeeRepository.GetDeliveryFeeById(request.DeliveryFeeId);

            if (existingDeliveryFee == null)
            {
                return null!;
            }

            existingDeliveryFee!.District = request.District.Trim() == "" ? existingDeliveryFee.District : request.District;
            existingDeliveryFee!.City = request.City.Trim() == "" ? existingDeliveryFee.City : request.City;
            existingDeliveryFee.FeeAmount = request.FeeAmount == 0 ? existingDeliveryFee.FeeAmount : request.FeeAmount;
            existingDeliveryFee.EstimatedDay = request.EstimatedDay == null ? existingDeliveryFee.EstimatedDay : (DateTime)request.EstimatedDay;
            existingDeliveryFee.EffectiveDay = request.EffectiveDay == null ? existingDeliveryFee.EffectiveDay : (DateTime)request.EffectiveDay;

            await deliveryFeeRepository.UpdateDeliveryFee(existingDeliveryFee);

            var updatedDeliveryFeeDto = new GetDeliveryFeeResponseDto
            {
                DeliveryFeeId = existingDeliveryFee.DeliveryFeeId,
                District = existingDeliveryFee.District,
                City = existingDeliveryFee.City,
                FeeAmount = existingDeliveryFee.FeeAmount,
                EstimatedDay = existingDeliveryFee.EstimatedDay,
                EffectiveDay = existingDeliveryFee.EffectiveDay
            };

            return updatedDeliveryFeeDto;
        }

        public async Task<bool> DeleteDeliveryFeeById(Guid deliveryFeeId)
        {
            var existingDeliveryFee = await deliveryFeeRepository.GetDeliveryFeeById(deliveryFeeId);

            if (existingDeliveryFee == null)
            {
                return false;
            }
            else
            {
                await deliveryFeeRepository.DeleteDeliveryFee(existingDeliveryFee);
                return true;
            }
        }
    }
}
