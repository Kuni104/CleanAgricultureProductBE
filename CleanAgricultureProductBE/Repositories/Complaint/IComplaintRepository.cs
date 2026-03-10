using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories.Complaint
{
    public interface IComplaintRepository
    {
        Task AddAsync(Models.Complaint complaint);
        Task<Models.Complaint?> GetByIdAsync(Guid complaintId);
        Task<Models.Complaint?> GetByOrderIdAsync(Guid orderId);
        Task<List<Models.Complaint>> GetAllAsync(int offset, int pageSize, string? keyword);
        Task<int> CountAllAsync(string? keyword);
        Task<List<Models.Complaint>> GetByCustomerIdAsync(Guid customerId, int offset, int pageSize);
        Task<int> CountByCustomerIdAsync(Guid customerId);
        Task UpdateAsync(Models.Complaint complaint);
        Task AddComplaintImageAsync(ComplaintImage image);
        Task SaveChangesAsync();
    }
}
