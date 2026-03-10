using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.CycleSchedule;

namespace CleanAgricultureProductBE.Services.CycleSchedule
{
    public interface ICycleScheduleService
    {
        public Task<ResponseDtoWithPagination<List<CycleScheduleResponseDto>>> GetCycleSchedules(int? page, int? size, string? keyword);
    }
}
