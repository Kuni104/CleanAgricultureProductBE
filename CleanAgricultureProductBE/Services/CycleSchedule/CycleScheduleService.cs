using CleanAgricultureProductBE.DTOs.CycleSchedule;
using CleanAgricultureProductBE.Repositories.CycleSchedule;

namespace CleanAgricultureProductBE.Services.CycleSchedule
{
    public class CycleScheduleService(ICycleScheduleRepository cycleScheduleRepository) : ICycleScheduleService
    {
        public async Task<List<CycleScheduleResponseDto>> GetCycleSchedules()
        {
            var cycleSchedules = await cycleScheduleRepository.GetCycleSchedules();

            if (cycleSchedules == null)
            {
                return null!;
            }

            List<CycleScheduleResponseDto> cycleScheduleResponseDtos = new List<CycleScheduleResponseDto>();

            foreach (var cycleSchedule in cycleSchedules)
            {
                CycleScheduleResponseDto cycleScheduleResponseDto = new CycleScheduleResponseDto
                {
                    CycleScheduleId = cycleSchedule.CycleScheduleId,
                    OrderId = cycleSchedule.OrderId,
                    DayCycle = cycleSchedule.DayCycle,
                    isMonthly = cycleSchedule.isMonthly,
                    StartAt = cycleSchedule.StartAt,
                    CreatedAt = cycleSchedule.CreatedAt,
                    UpdatedAt = cycleSchedule.UpdatedAt,
                    Status = cycleSchedule.Status
                };
                cycleScheduleResponseDtos.Add(cycleScheduleResponseDto);
            }

            return cycleScheduleResponseDtos;

        }
    }
}
