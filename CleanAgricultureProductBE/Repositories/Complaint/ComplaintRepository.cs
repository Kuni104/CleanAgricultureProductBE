using CleanAgricultureProductBE.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.Complaint
{
    public class ComplaintRepository(AppDbContext context) : IComplaintRepository
    {
        public async Task AddAsync(Models.Complaint complaint)
        {
            context.Complaints.Add(complaint);
            await context.SaveChangesAsync();
        }

        public async Task<Models.Complaint?> GetByIdAsync(Guid complaintId)
        {
            return await context.Complaints
                .Include(c => c.Order)
                .Include(c => c.Staff)
                .Include(c => c.ProductComplaints)
                    .ThenInclude(pc => pc.Product)
                .FirstOrDefaultAsync(c => c.ComplaintId == complaintId);
        }

        public async Task<Models.Complaint?> GetByOrderIdAsync(Guid orderId)
        {
            return await context.Complaints
                .Include(c => c.ProductComplaints)
                    .ThenInclude(pc => pc.Product)
                .FirstOrDefaultAsync(c => c.OrderId == orderId);
        }

        public async Task<List<Models.Complaint>> GetAllAsync(int offset, int pageSize, string? keyword)
        {
            var query = context.Complaints
                .Include(c => c.Order)
                    .ThenInclude(o => o.Customer)
                .Include(c => c.Staff)
                .Include(c => c.ProductComplaints)
                    .ThenInclude(pc => pc.Product)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(c => c.Subject.Contains(keyword) || c.Description.Contains(keyword));

            return await query.OrderByDescending(c => c.CreatedAt).Skip(offset).Take(pageSize).ToListAsync();
        }

        public async Task<int> CountAllAsync(string? keyword)
        {
            var query = context.Complaints.AsQueryable();
            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(c => c.Subject.Contains(keyword) || c.Description.Contains(keyword));
            return await query.CountAsync();
        }

        public async Task<List<Models.Complaint>> GetByCustomerIdAsync(Guid customerId, int offset, int pageSize)
        {
            return await context.Complaints
                .Include(c => c.Order)
                .Include(c => c.Staff)
                .Include(c => c.ProductComplaints)
                    .ThenInclude(pc => pc.Product)
                .Where(c => c.Order.CustomerId == customerId)
                .OrderByDescending(c => c.CreatedAt)
                .Skip(offset).Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountByCustomerIdAsync(Guid customerId)
        {
            return await context.Complaints.CountAsync(c => c.Order.CustomerId == customerId);
        }

        public async Task UpdateAsync(Models.Complaint complaint)
        {
            context.Complaints.Update(complaint);
            await context.SaveChangesAsync();
        }
    }
}
