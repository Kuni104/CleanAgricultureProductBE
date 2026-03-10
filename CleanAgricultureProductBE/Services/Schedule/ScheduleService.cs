using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.DTOs.Schedule;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Repositories.DSchedule;
using CleanAgricultureProductBE.Services.Schedule;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _repository;
    private readonly IAccountRepository _accountRepository;

    public ScheduleService(IScheduleRepository repository, IAccountRepository accountRepository)
    {
        _repository = repository;
        _accountRepository = accountRepository;
    }

    private ScheduleResponseDto MapToDto(Schedule schedule)
    {
        return new ScheduleResponseDto
        {
            ScheduleId = schedule.ScheduleId,
            DeliveryPersonId = schedule.DeliveryPersonId,
            ScheduledDate = schedule.ScheduledDate,
            Status = schedule.Status,
            TotalOrders = schedule.Orders.Count,
            CreatedAt = schedule.CreatedAt,
            UpdatedAt = schedule.UpdatedAt
        };
    }

    public async Task<Guid> CreateScheduleAsync(Guid deliveryPersonId, DateTime scheduledDate)
    {
        var deliveryPerson = await _accountRepository.GetByIdAsync(deliveryPersonId);

        if (deliveryPerson == null)
            throw new Exception("DeliveryPersonId không tồn tại");

        if (deliveryPerson.RoleId != 4)
            throw new Exception("Tài khoản này không phải là nhân viên giao hàng");

        var todayDate = DateTime.UtcNow.Date;
        scheduledDate = scheduledDate.Date;

        if (scheduledDate < todayDate)
            throw new Exception("Không được tạo lịch trong quá khứ");

        var existingSchedule = await _repository
            .GetByDeliveryPersonAndDateAsync(deliveryPersonId, scheduledDate);

        if (existingSchedule != null)
            throw new Exception("Delivery person already has a schedule on this date");

        var schedule = new Schedule
        {
            ScheduleId = Guid.NewGuid(),
            DeliveryPersonId = deliveryPersonId,
            ScheduledDate = scheduledDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Status = "Pending"
        };

        await _repository.AddAsync(schedule);
        await _repository.SaveChangesAsync();

        return schedule.ScheduleId;
    }

    public async Task AssignOrdersAsync(Guid scheduleId, List<Guid> orderIds)
    {
        var schedule = await _repository.GetByIdAsync(scheduleId);

        if (schedule == null)
            throw new Exception("Không tìm thấy lịch");

        var orders = await _repository.GetOrdersByIdsAsync(orderIds);

        if (!orders.Any())
            throw new Exception("Không tìm thấy order");

        foreach (var order in orders)
        {
            if (order.ScheduleId != null)
                throw new Exception($"Order {order.OrderId} already assigned to a schedule");

            order.ScheduleId = schedule.ScheduleId;
        }

        schedule.UpdatedAt = DateTime.UtcNow;

        await _repository.SaveChangesAsync();
    }

    public async Task<ResponseDtoWithPagination<List<ScheduleResponseDto>>> GetSchedulesOfDeliveryPerson(string accountEmail, int? page, int? size, string? keyword)
    {
        bool isPagination = false;
        int offset = 0;
        int pageSize = 0;
        if (page != null && size != null)
        {
            pageSize = (int)size;
            offset = (int)((page - 1) * size);
            isPagination = true;
        }

        var account = await _accountRepository.GetByEmailAsync(accountEmail);

        var schedules = await _repository.GetByDeliveryPerson(account!.AccountId);

        var totalItems = schedules.Count();

        if (isPagination)
        {
            schedules = await _repository.GetByDeliveryPersonWithPagination(account!.AccountId, offset, pageSize);
        }

        List<ScheduleResponseDto> scheduleDtos = new List<ScheduleResponseDto>();

        foreach (var schedule in schedules)
        {
            scheduleDtos.Add(new ScheduleResponseDto
            {
                ScheduleId = schedule.ScheduleId,
                DeliveryPersonId = schedule.DeliveryPersonId,
                ScheduledDate = schedule.ScheduledDate,
                Status = schedule.Status,
                TotalOrders = schedule.Orders.Count,
                CreatedAt = schedule.CreatedAt,
                UpdatedAt = schedule.UpdatedAt
            });
        }

        var result = new ResponseDtoWithPagination<List<ScheduleResponseDto>>
        {
            ResultObject = scheduleDtos
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