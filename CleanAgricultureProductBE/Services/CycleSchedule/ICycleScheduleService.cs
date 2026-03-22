using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.CycleSchedule;

namespace CleanAgricultureProductBE.Services.CycleSchedule
{
    public interface ICycleScheduleService
    {
        public Task<ResponseDtoWithPagination<List<CycleScheduleResponseDto>>> GetCycleSchedules(string accountEmail,int? page, int? size);
        public Task<ResponseDtoWithPagination<List<CycleScheduleResponseDto>>> GetCycleSchedulesAdmin(int? page, int? size);
        public Task<CycleScheduleResponseDto> GetCycleScheduleByIdAdmin(Guid cycleScheduleId);
        public Task<CycleScheduleResponseDto> GetCycleScheduleById(string accountEmail, Guid cycleScheduleId);
        public Task<CycleScheduleResponseDto> CancelCycleScheduleById(string accountEmail,Guid cycleScheduleId);
        public Task<CycleScheduleResponseDto> CancelCycleScheduleByIdAdmin(Guid cycleScheduleId);
    }
}
