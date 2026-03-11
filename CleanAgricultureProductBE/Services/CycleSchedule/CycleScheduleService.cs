using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.CycleSchedule;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Repositories.CycleSchedule;

namespace CleanAgricultureProductBE.Services.CycleSchedule
{
    public class CycleScheduleService(ICycleScheduleRepository cycleScheduleRepository) : ICycleScheduleService
    {
        public async Task<ResponseDtoWithPagination<List<CycleScheduleResponseDto>>> GetCycleSchedules(int? page, int? size, string? keyword)
        {
            bool isPagination = false;
            int offset = 0;
            int pageSize = 0;
            if (page != null && size != null)
            {
                if (page <= 0)
                    throw new ArgumentException("Số trang (page) phải lớn hơn 0");
                if (size <= 0)
                    throw new ArgumentException("Kích thước trang (size) phải lớn hơn 0");

                pageSize = (int)size;
                offset = (int)((page - 1) * size);
                isPagination = true;
            }

            var cycleSchedules = await cycleScheduleRepository.GetCycleSchedules();

            if (cycleSchedules == null)
            {
                return null!;
            }

            int totalItems = cycleSchedules.Count();

            if (isPagination)
            {
                cycleSchedules = cycleSchedules.Skip(offset).Take(pageSize).ToList();
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

            var result = new ResponseDtoWithPagination<List<CycleScheduleResponseDto>>
            {
                ResultObject = cycleScheduleResponseDtos
            };

            if (isPagination)
            {
                int totalPage = totalItems / pageSize + (totalItems % pageSize == 0 ? 1 : 0);
                if (totalItems < pageSize || page <= size)
                {
                    totalPage = 1;
                }

                result.Pagination = new Pagination
                {
                    PageNumber = (int)page!,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPage
                };
            }

            return result;

        }
    }
}
