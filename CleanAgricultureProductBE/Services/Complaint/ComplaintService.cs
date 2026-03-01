using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Complaint;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Repositories.Complaint;
using CleanAgricultureProductBE.Repositories.Order;

namespace CleanAgricultureProductBE.Services.Complaint
{
    public class ComplaintService(
        IComplaintRepository complaintRepository,
        IOrderRepository orderRepository,
        IAccountRepository accountRepository) : IComplaintService
    {
        public async Task<ComplaintResponseDto> CreateComplaintAsync(string accountEmail, CreateComplaintRequestDto request)
        {
            var account = await accountRepository.GetByEmailAsync(accountEmail)
                ?? throw new Exception("Account not found");

            var order = await orderRepository.GetOrderByOrderId(request.OrderId)
                ?? throw new Exception("Order not found");

            if (order.CustomerId != account.UserProfile.UserProfileId)
                throw new UnauthorizedAccessException("This order does not belong to you");

            var existing = await complaintRepository.GetByOrderIdAsync(request.OrderId);
            if (existing != null)
                throw new InvalidOperationException("A complaint for this order already exists");

            var complaint = new Models.Complaint
            {
                ComplaintId = Guid.NewGuid(),
                OrderId = request.OrderId,
                Subject = request.Subject,
                Description = request.Description,
                Evidence = request.Evidence ?? string.Empty,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            if (request.ProductIds != null && request.ProductIds.Count > 0)
            {
                foreach (var productId in request.ProductIds.Distinct())
                {
                    complaint.ProductComplaints.Add(new ProductComplaint
                    {
                        ProductComplaintId = Guid.NewGuid(),
                        ProductId = productId
                    });
                }
            }

            await complaintRepository.AddAsync(complaint);

            var saved = await complaintRepository.GetByIdAsync(complaint.ComplaintId);
            return MapToDto(saved!);
        }

        public async Task<ResponseDtoWithPagination<List<ComplaintResponseDto>>> GetMyComplaintsAsync(string accountEmail, int? page, int? size)
        {
            var account = await accountRepository.GetByEmailAsync(accountEmail)
                ?? throw new Exception("Account not found");

            int pageSize = size ?? 10;
            int pageNumber = page ?? 1;
            int offset = (pageNumber - 1) * pageSize;

            var customerId = account.UserProfile.UserProfileId;
            var complaints = await complaintRepository.GetByCustomerIdAsync(customerId, offset, pageSize);
            var total = await complaintRepository.CountByCustomerIdAsync(customerId);

            return new ResponseDtoWithPagination<List<ComplaintResponseDto>>
            {
                ResultObject = complaints.Select(MapToDto).ToList(),
                Pagination = new Pagination { PageNumber = pageNumber, PageSize = pageSize, TotalItems = total, TotalPages = (int)Math.Ceiling((double)total / pageSize) }
            };
        }

        public async Task<ResponseDtoWithPagination<List<ComplaintResponseDto>>> GetAllComplaintsAsync(int? page, int? size, string? keyword)
        {
            int pageSize = size ?? 10;
            int pageNumber = page ?? 1;
            int offset = (pageNumber - 1) * pageSize;

            var complaints = await complaintRepository.GetAllAsync(offset, pageSize, keyword);
            var total = await complaintRepository.CountAllAsync(keyword);

            return new ResponseDtoWithPagination<List<ComplaintResponseDto>>
            {
                ResultObject = complaints.Select(MapToDto).ToList(),
                Pagination = new Pagination { PageNumber = pageNumber, PageSize = pageSize, TotalItems = total, TotalPages = (int)Math.Ceiling((double)total / pageSize) }
            };
        }

        public async Task<ComplaintResponseDto?> GetComplaintByIdAsync(Guid complaintId)
        {
            var complaint = await complaintRepository.GetByIdAsync(complaintId);
            return complaint == null ? null : MapToDto(complaint);
        }

        private static ComplaintResponseDto MapToDto(Models.Complaint c) => new()
        {
            ComplaintId = c.ComplaintId,
            OrderId = c.OrderId,
            Subject = c.Subject,
            Description = c.Description,
            Evidence = c.Evidence,
            Status = c.Status,
            CreatedAt = c.CreatedAt,
            ResolveAt = c.ResolveAt,
            StaffName = c.Staff != null ? $"{c.Staff.UserProfile?.FirstName} {c.Staff.UserProfile?.LastName}".Trim() : null,
            ProductComplaints = c.ProductComplaints.Select(pc => new ProductComplaintResponseDto
            {
                ProductComplaintId = pc.ProductComplaintId,
                ProductId = pc.ProductId,
                ProductName = pc.Product?.Name ?? string.Empty
            }).ToList()
        };
    }
}
