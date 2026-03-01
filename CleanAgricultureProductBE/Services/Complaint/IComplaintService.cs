using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Complaint;

namespace CleanAgricultureProductBE.Services.Complaint
{
    public interface IComplaintService
    {
        // Customer
        Task<ComplaintResponseDto> CreateComplaintAsync(string accountEmail, CreateComplaintRequestDto request);
        Task<ResponseDtoWithPagination<List<ComplaintResponseDto>>> GetMyComplaintsAsync(string accountEmail, int? page, int? size);

        // Staff
        Task<ResponseDtoWithPagination<List<ComplaintResponseDto>>> GetAllComplaintsAsync(int? page, int? size, string? keyword);
        Task<ComplaintResponseDto?> GetComplaintByIdAsync(Guid complaintId);
    }
}
