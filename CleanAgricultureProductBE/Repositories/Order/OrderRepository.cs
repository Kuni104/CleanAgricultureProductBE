
using CleanAgricultureProductBE.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.Order
{
    public class OrderRepository(AppDbContext context) : IOrderRepository
    {
        public async Task<Models.Order?> GetOrderByOrderId(Guid orderId)
        {
            return await context.Orders.Where(o => o.OrderId == orderId)
                                       .Include(o => o.Address)
                                       .Include(o => o.Payment)
                                       .Include(o => o.Schedule)
                                       .Include(o => o.Customer)
                                       .ThenInclude(c => c.Account)
                                       .FirstOrDefaultAsync();
        }
        public async Task AddOrder(Models.Order order)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();
        }

        public async Task<List<Models.Order>> GetOrdersByCustomerId(Guid customerId)
        {
            return await context.Orders.Where(o => o.CustomerId == customerId)
                                       .Include(o => o.Payment)
                                       .Include(o => o.Schedule)
                                       .Include(o => o.Address)
                                       .OrderByDescending(o => o.OrderDate)
                                       .ToListAsync();
        }

        public async Task<List<Models.Order>> GetOrdersByCustomerIdWithPagination(Guid customerId, int offset, int pageSize)
        {
            return await context.Orders.Where(o => o.CustomerId == customerId)
                                       .Include(o => o.Payment)
                                       .Include(o => o.Schedule)
                                       .Include(o => o.Address)
                                       .OrderByDescending(o => o.OrderDate)
                                       .Skip(offset)
                                       .Take(pageSize)
                                       .ToListAsync();
        }

    }
}
