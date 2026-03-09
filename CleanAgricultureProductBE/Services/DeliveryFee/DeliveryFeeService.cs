using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.DeliveryFee;
using CleanAgricultureProductBE.Repositories.DeliveryFee;

namespace CleanAgricultureProductBE.Services.DeliveryFee
{
    public class DeliveryFeeService(IDeliveryFeeRepository deliveryFeeRepository) : IDeliveryFeeService
    {
        public async Task<List<DeliveryFeeResponseDto>> GetDeliveryFeeList()
        {
            var deliveryFees = await deliveryFeeRepository.GetDeliveryFeeList();

            var deliveryFeeList = deliveryFees.Select(df => new DeliveryFeeResponseDto
            {
                DeliveryFeeId = df.DeliveryFeeId,
                FromKilometer = df.FromKilometer,
                ToKilometer = df.ToKilometer,
                FeeAmount = df.FeeAmount
            }).ToList();

            return deliveryFeeList;
        }

        public async Task<DeliveryFeeResponseDto> GetDeliveryFeeById(Guid deliveryFeeId)
        {
            var deliveryFee = await deliveryFeeRepository.GetDeliveryFeeById(deliveryFeeId);

            return new DeliveryFeeResponseDto
            {
                DeliveryFeeId = deliveryFee!.DeliveryFeeId,
                FromKilometer = deliveryFee!.FromKilometer,
                ToKilometer = deliveryFee!.ToKilometer,
                FeeAmount = deliveryFee!.FeeAmount
            };
        }

        public async Task<DeliveryFeeResponseDto> AddDeliveryFee(CreateDeliveryFeeRequestDto request)
        {
            var newDeliveryFee = new Models.DeliveryFee
            {
                DeliveryFeeId = Guid.NewGuid(),
                FromKilometer = request.FromKilometer,
                ToKilometer = request.ToKilometer,
                FeeAmount = request.FeeAmount
            };

            await deliveryFeeRepository.AddDeliveryFee(newDeliveryFee);

            var deliveryFeeDto = new DeliveryFeeResponseDto
            {
                DeliveryFeeId = newDeliveryFee.DeliveryFeeId,
                FromKilometer = newDeliveryFee.FromKilometer,
                ToKilometer = newDeliveryFee.ToKilometer,
                FeeAmount = newDeliveryFee.FeeAmount
            };

            return deliveryFeeDto;
        }

        public async Task<ResultStatusWithData<DeliveryFeeResponseDto>> UpdateDeliveryFee(Guid deliveryFeeId, UpdateDeliveryFeeRequestDto request)
        {
            var existingDeliveryFee = await deliveryFeeRepository.GetDeliveryFeeById(deliveryFeeId);

            if (existingDeliveryFee == null)
            {
                return null!;
            }

            existingDeliveryFee.FromKilometer = request.FromKilometer == null ? existingDeliveryFee.FromKilometer : (decimal)request.FromKilometer;
            existingDeliveryFee.ToKilometer = request.ToKilometer == null ? existingDeliveryFee.ToKilometer : (decimal)request.ToKilometer;

            var checkValidRequest = await deliveryFeeRepository.CheckDeliveryFeeInBetween(existingDeliveryFee.FromKilometer, existingDeliveryFee.ToKilometer);

            if (!checkValidRequest)
            {
                return new ResultStatusWithData<DeliveryFeeResponseDto>
                {
                    Status = "Invalid Request",
                    Data = null,
                };
            }

            existingDeliveryFee.FeeAmount = request.FeeAmount == null ? existingDeliveryFee.FeeAmount : (decimal)request.FeeAmount;

            await deliveryFeeRepository.UpdateDeliveryFee(existingDeliveryFee);

            var updatedDeliveryFeeDto = new DeliveryFeeResponseDto
            {
                DeliveryFeeId = existingDeliveryFee.DeliveryFeeId,
                FromKilometer = existingDeliveryFee.FromKilometer,
                ToKilometer = existingDeliveryFee.ToKilometer,
                FeeAmount = existingDeliveryFee.FeeAmount,
            };

            return new ResultStatusWithData<DeliveryFeeResponseDto>
            {
                Status = "ok",
                Data = updatedDeliveryFeeDto
            };
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
