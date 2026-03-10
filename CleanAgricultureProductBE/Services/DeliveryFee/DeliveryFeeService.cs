using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.DeliveryFee;
using CleanAgricultureProductBE.Repositories.DeliveryFee;
using System.Management;

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
                City = deliveryFee.City,
                District = deliveryFee.District,
                Ward = deliveryFee.Ward,
                FeeAmount = deliveryFee!.FeeAmount
            };
        }

        public async Task<DeliveryFeeResponseDto> AddDeliveryFee(CreateDeliveryFeeRequestDto request)
        {
            var checkValidRequest = await deliveryFeeRepository.CheckDeliveryFee(request.City, request.Ward, request.District);
            if (checkValidRequest == true)
            {
                return null!;
            }


            var newDeliveryFee = new Models.DeliveryFee
            {
                DeliveryFeeId = Guid.NewGuid(),
                City = request.City,
                District = request.District,
                Ward = request.Ward,
                FeeAmount = request.FeeAmount
            };

            await deliveryFeeRepository.AddDeliveryFee(newDeliveryFee);

            var deliveryFeeDto = new DeliveryFeeResponseDto
            {
                DeliveryFeeId = newDeliveryFee.DeliveryFeeId,
                City = newDeliveryFee.City,
                District = newDeliveryFee.District,
                Ward = newDeliveryFee.Ward,
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

            existingDeliveryFee.City = string.IsNullOrEmpty(request.City) ? existingDeliveryFee.City : request.City;
            existingDeliveryFee.City = string.IsNullOrEmpty(request.Ward) ? existingDeliveryFee.Ward : request.Ward;
            existingDeliveryFee.City = string.IsNullOrEmpty(request.District) ? existingDeliveryFee.District : request.District;

            existingDeliveryFee.FeeAmount = request.FeeAmount == null ? existingDeliveryFee.FeeAmount : (decimal)request.FeeAmount;

            var checkValidRequest = await deliveryFeeRepository.CheckDeliveryFee(existingDeliveryFee.City, existingDeliveryFee.Ward, existingDeliveryFee.District);

            if (!checkValidRequest)
            {
                return new ResultStatusWithData<DeliveryFeeResponseDto>
                {
                    Status = "Invalid Request",
                    Data = null,
                };
            }

            await deliveryFeeRepository.UpdateDeliveryFee(existingDeliveryFee);

            var updatedDeliveryFeeDto = new DeliveryFeeResponseDto
            {
                DeliveryFeeId = existingDeliveryFee.DeliveryFeeId,
                City = existingDeliveryFee.City,
                Ward = existingDeliveryFee.Ward,
                District = existingDeliveryFee.District,
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
