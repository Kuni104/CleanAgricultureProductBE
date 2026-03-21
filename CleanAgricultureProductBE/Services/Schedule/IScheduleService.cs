using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Schedule;

namespace CleanAgricultureProductBE.Services.Schedule
{
    public interface IScheduleService
    {
        Task<Guid> CreateScheduleAsync(Guid deliveryPersonId, DateTime scheduledDate);
        Task AssignOrdersAsync(Guid scheduleId, List<Guid> orderIds);

        public Task<ResultStatusWithData<ScheduleResponseDto>> AssignDeliveryPersonToSchedule(Guid scheduleId, Guid deliveryPersonId);

        public Task<ResponseDtoWithPagination<List<ScheduleResponseDto>>> GetSchedulesOfDeliveryPerson(string accountEmail, int? page, int? size, string? keyword);
        public Task<ScheduleResponseDto> GetSchedulesOfDeliveryPersonToday(string accountEmail);

        //Admin
        public Task<ResultStatusWithData<ScheduleResponseDto>> GetSchedulesTodayByDeliveryPerson(Guid deliveryPersonId);
        public Task<ResultStatusWithData<ScheduleResponseDto>> GetSchedulesByDateByDeliveryPerson(Guid deliveryPersonId, DateTime dateTime);
        public Task<ResponseDtoWithPagination<List<ScheduleResponseDto>>> GetSchedulesByDate(DateTime dateTime, int? page, int? size);
        public Task<ResponseDtoWithPagination<List<ScheduleResponseDto>>> GetSchedulesToday(int? page, int? size);
    }
}
