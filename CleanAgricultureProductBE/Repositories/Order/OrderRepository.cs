
using CleanAgricultureProductBE.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.Order
{
    public class OrderRepository(AppDbContext context) : IOrderRepository
    {
        public async Task AddOrder(Models.Order order)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();
        }

        public async Task<List<Models.Order>> GetOrdersByCustomerId(Guid customerId)
        {
            return await context.Orders.Where(o => o.CustomerId == customerId)
                                       .Include(o => o.Payment)
                                       .OrderByDescending(o => o.OrderDate)
                                       .ToListAsync();
        }

        public async Task<List<Models.Order>> GetOrdersByCustomerIdWithPagination(Guid customerId, int offset, int pageSize)
        {
            return await context.Orders.Where(o => o.CustomerId == customerId)
                                       .Include(o => o.Payment)
                                       .OrderByDescending(o => o.OrderDate)
                                       .Skip(offset)
                                       .Take(pageSize)
                                       .ToListAsync();
        }

    }
}
