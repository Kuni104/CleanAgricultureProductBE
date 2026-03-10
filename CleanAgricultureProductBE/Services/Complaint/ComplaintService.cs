using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Complaint;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Repositories.Complaint;
using CleanAgricultureProductBE.Repositories.Order;
using CleanAgricultureProductBE.Services.Image;

namespace CleanAgricultureProductBE.Services.Complaint
{
    public class ComplaintService(
        IComplaintRepository complaintRepository,
        IOrderRepository orderRepository,
        IAccountRepository accountRepository,
        ICloudinaryService _cloudinaryService) : IComplaintService
    {
        public async Task<ComplaintResponseDto> CreateComplaintAsync(string accountEmail, CreateComplaintRequestDto request)
        {
            if (request.OrderId == Guid.Empty)
                throw new ArgumentException("OrderId không được để trống");

            if (string.IsNullOrWhiteSpace(request.Subject))
                throw new ArgumentException("Subject không được để trống");

            if (string.IsNullOrWhiteSpace(request.Description))
                throw new ArgumentException("Description không được để trống");

            if (string.IsNullOrWhiteSpace(request.Evidence))
                throw new ArgumentException("Evidence không được để trống");


            if (request.ProductIds == null || !request.ProductIds.Any())
                throw new ArgumentException("Phải chọn ít nhất 1 sản phẩm");

            if (request.Images != null && request.Images.Any())
            {
                if (request.Images.Count > 5)
                    throw new ArgumentException("Chỉ được upload tối đa 5 ảnh");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                foreach (var file in request.Images)
                {
                    if (file.Length == 0)
                        throw new ArgumentException("File ảnh không hợp lệ");

                    var extension = Path.GetExtension(file.FileName).ToLower();

                    if (!allowedExtensions.Contains(extension))
                        throw new ArgumentException("Chỉ chấp nhận file jpg, jpeg, png, webp");

                    if (file.Length > 5 * 1024 * 1024)
                        throw new ArgumentException("Ảnh phải nhỏ hơn 5MB");
                }
            }

            var account = await accountRepository.GetByEmailAsync(accountEmail)
                ?? throw new Exception("Account không tồn tại");

            var order = await orderRepository.GetOrderByOrderId(request.OrderId)
                ?? throw new Exception("Order không tồn tại");

 
            if (order.CustomerId != account.UserProfile.UserProfileId)
                throw new UnauthorizedAccessException("Đơn hàng này không thuộc về bạn");

            var existing = await complaintRepository.GetByOrderIdAsync(request.OrderId);
            if (existing != null)
                throw new InvalidOperationException("Đơn hàng này đã được khiếu nại");

            var complaint = new Models.Complaint
            {
                ComplaintId = Guid.NewGuid(),
                OrderId = request.OrderId,
                Subject = request.Subject.Trim(),
                Description = request.Description.Trim(),
                Evidence = request.Evidence.Trim(),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                Images = new List<ComplaintImage>()
            };

            foreach (var productId in request.ProductIds.Distinct())
            {
                complaint.ProductComplaints.Add(new ProductComplaint
                {
                    ProductComplaintId = Guid.NewGuid(),
                    ProductId = productId,
                    ComplaintId = complaint.ComplaintId
                });
            }

            if (request.Images != null && request.Images.Any())
            {
                foreach (var image in request.Images)
                {
                    var url = await _cloudinaryService.UploadImageAsync(image, "complaints");

                    if (!string.IsNullOrEmpty(url))
                    {
                        complaint.Images.Add(new ComplaintImage
                        {
                            Id = Guid.NewGuid(),
                            ComplaintId = complaint.ComplaintId,
                            ImageUrl = url
                        });
                    }
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
            StaffName = c.Staff != null
        ? $"{c.Staff.UserProfile?.FirstName} {c.Staff.UserProfile?.LastName}".Trim()
        : null,

            ProductComplaints = c.ProductComplaints.Select(pc => new ProductComplaintResponseDto
            {
                ProductComplaintId = pc.ProductComplaintId,
                ProductId = pc.ProductId,
                ProductName = pc.Product?.Name ?? string.Empty
            }).ToList(),

            Images = c.Images?.Select(i => new ComplaintImageResponseDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl
            }).ToList() ?? new List<ComplaintImageResponseDto>()
        };
    }
}
