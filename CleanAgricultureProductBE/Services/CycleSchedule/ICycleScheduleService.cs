using CleanAgricultureProductBE.DTOs.CycleSchedule;

namespace CleanAgricultureProductBE.Services.CycleSchedule
{
    public interface ICycleScheduleService
    {
        public Task<List<CycleScheduleResponseDto>> GetCycleSchedules();
    }
}
