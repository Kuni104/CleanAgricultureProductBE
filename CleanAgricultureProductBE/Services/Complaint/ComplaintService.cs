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
                throw new ArgumentException("Mã đơn hàng không được để trống");

            if (string.IsNullOrWhiteSpace(request.Subject))
                throw new ArgumentException("Tiêu đề không được để trống");

            if (string.IsNullOrWhiteSpace(request.Description))
                throw new ArgumentException("Mô tả không được để trống");

            if (request.Subject.Length > 200)
                throw new ArgumentException("Tiêu đề không được vượt quá 200 ký tự");

            if (request.Description.Length > 2000)
                throw new ArgumentException("Mô tả không được vượt quá 2000 ký tự");

            // Bắt buộc phải có bằng chứng: ảnh hoặc evidence text
            bool hasImages = request.Images != null && request.Images.Any();
            bool hasEvidence = !string.IsNullOrWhiteSpace(request.Evidence);
            if (!hasImages && !hasEvidence)
                throw new ArgumentException("Bằng chứng là bắt buộc. Vui lòng cung cấp ảnh hoặc mô tả bằng chứng");

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
                        throw new ArgumentException("Chỉ chấp nhận ảnh định dạng jpg, jpeg, png, webp");

                    if (file.Length > 5 * 1024 * 1024)
                        throw new ArgumentException("Dung lượng ảnh không được vượt quá 5MB");
                }
            }

            var account = await accountRepository.GetByEmailAsync(accountEmail)
                ?? throw new Exception("Không tìm thấy tài khoản");

            var order = await orderRepository.GetOrderByOrderId(request.OrderId)
                ?? throw new Exception("Không tìm thấy đơn hàng");

 
            if (order.CustomerId != account.UserProfile.UserProfileId)
                throw new UnauthorizedAccessException("Đơn hàng này không thuộc về bạn");

            if (order.OrderStatus != "Completed")
                throw new InvalidOperationException("Chỉ có thể khiếu nại đơn hàng đã hoàn thành (Completed)");

            var existing = await complaintRepository.GetByOrderIdAsync(request.OrderId);
            if (existing != null)

                throw new InvalidOperationException("Đơn hàng này đã có khiếu nại");

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
                ?? throw new Exception("Không tìm thấy tài khoản");

            int pageSize = size ?? 10;
            int pageNumber = page ?? 1;

            if (pageNumber <= 0)
                throw new ArgumentException("Số trang (page) phải lớn hơn 0");
            if (pageSize <= 0)
                throw new ArgumentException("Kích thước trang (size) phải lớn hơn 0");

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

            if (pageNumber <= 0)
                throw new ArgumentException("Số trang (page) phải lớn hơn 0");
            if (pageSize <= 0)
                throw new ArgumentException("Kích thước trang (size) phải lớn hơn 0");

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

        public async Task<ComplaintResponseDto> UpdateComplaintStatusAsync(string staffEmail, Guid complaintId, UpdateComplaintStatusDto dto)
        {
            var staffAccount = await accountRepository.GetByEmailAsync(staffEmail)
                ?? throw new Exception("Không tìm thấy tài khoản");

            var complaint = await complaintRepository.GetByIdAsync(complaintId)
                ?? throw new Exception("Không tìm thấy khiếu nại");

            if (complaint.Status != "Pending")
                throw new InvalidOperationException($"Không thể cập nhật khiếu nại đang ở trạng thái '{complaint.Status}'");

            var validStatuses = new[] { "Resolved", "Rejected" };
            if (!validStatuses.Contains(dto.Status))
                throw new Exception("Trạng thái không hợp lệ. Chỉ chấp nhận: Resolved, Rejected");

            if (dto.Status == "Resolved")
            {
                var validResolutions = new[] { "Exchange", "Refund" };
                if (string.IsNullOrWhiteSpace(dto.Resolution) || !validResolutions.Contains(dto.Resolution))
                    throw new Exception("Khi xử lý khiếu nại (Resolved), phải chọn hình thức: Exchange (đổi hàng) hoặc Refund (hoàn tiền)");

                complaint.Resolution = dto.Resolution;
            }

            complaint.Status = dto.Status;
            complaint.StaffId = staffAccount.AccountId;
            complaint.ResolveAt = DateTime.UtcNow;

            await complaintRepository.UpdateAsync(complaint);

            var updated = await complaintRepository.GetByIdAsync(complaintId);
            return MapToDto(updated!);
        }

        private static ComplaintResponseDto MapToDto(Models.Complaint c) => new()
        {
            ComplaintId = c.ComplaintId,
            OrderId = c.OrderId,
            Subject = c.Subject,
            Description = c.Description,
            Evidence = c.Evidence,
            Status = c.Status,
            Resolution = c.Resolution,
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
