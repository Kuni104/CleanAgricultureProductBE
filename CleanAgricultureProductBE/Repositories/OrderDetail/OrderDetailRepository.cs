using CleanAgricultureProductBE.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.OrderDetail
{
    public class OrderDetailRepository(AppDbContext context) : IOrderDetailRepository
    {
        public async Task AddOrderDetails(List<Models.OrderDetail> orderDetails)
        {
            context.OrderDetails.AddRange(orderDetails);
            await context.SaveChangesAsync();
        }

        public async Task<List<Models.OrderDetail>> GetOrderDetailsByOrderId(Guid orderId)
        {
            return await context.OrderDetails.Where(od => od.OrderId == orderId)
                                            .Include(od => od.Product)
                                            .OrderByDescending(od => od.CreatedAt)
                                            .ToListAsync();
        }

        public async Task<List<Models.OrderDetail>> GetOrderDetailsByOrderIdWithPagination(Guid orderId, int offset, int pageSize, string? keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                return await context.OrderDetails
                                            .Where(od => od.Product.Name.Trim().ToLower().Contains(keyword.Trim().ToLower()))
                                            .Where(od => od.OrderId == orderId)
                                            .Include(od => od.Product)
                                            .OrderByDescending(od => od.CreatedAt)
                                            .Skip(offset)
                                            .Take(pageSize)
                                            .ToListAsync();
            }
            else
            {
                return await context.OrderDetails.Where(od => od.OrderId == orderId)
                                            .Include(od => od.Product)
                                            .OrderByDescending(od => od.CreatedAt)
                                            .Skip(offset)
                                            .Take(pageSize)
                                            .ToListAsync();
            }
        }
    }
}
