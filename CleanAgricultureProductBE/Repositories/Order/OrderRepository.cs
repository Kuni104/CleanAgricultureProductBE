using System;
using System.Threading.Tasks;
using CleanAgricultureProductBE.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.Order
{
    public class OrderRepository(AppDbContext context) : IOrderRepository
    {
        public async Task<List<Models.Order>> GetAllOrders()
        {
            return await context.Orders.Include(o => o.Address)
                                       .Include(o => o.Payment)
                                       .Include(o => o.Schedule)
                                       .Include(o => o.Customer)
                                       .ThenInclude(c => c.Account)
                                       .ToListAsync();
        }

        public async Task UpdateOrder(Models.Order order)
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }

        public async Task<List<Models.Order>> GetAllOrdersWithPagination(int offset, int pageSize)
        {
            return await context.Orders.Include(o => o.Address)
                                       .Include(o => o.Payment)
                                       .Include(o => o.Schedule)
                                       .Include(o => o.Customer)
                                       .ThenInclude(c => c.Account)
                                       .OrderByDescending(o => o.OrderDate)
                                       .Skip(offset)
                                       .Take(pageSize)
                                       .ToListAsync();
        }

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

        public async Task<Models.Order> GetOrderById(Guid orderId)
        {
            return await context.Orders.FindAsync(orderId);
        }

        public Task UpdateAsync(Models.Order order)
        {
            context.Orders.Update(order);
            return context.SaveChangesAsync();
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

        public async Task TestUpdateOrderWithPayment(Models.Order order)
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }

        public async Task<List<Models.Order>> GetAllOrdersInSchedule(Guid scheduleId)
        {
            return await context.Orders.Include(o => o.Address)
                                       .Include(o => o.Payment)
                                       .Include(o => o.Schedule)
                                       .Include(o => o.Customer)
                                       .ThenInclude(c => c.Account)
                                       .Where(o => o.ScheduleId == scheduleId)
                                       .ToListAsync();
        }

        public async Task<List<Models.Order>> GetAllOrdersInScheduleWithPagination(Guid scheduleId, int offset, int pageSize)
        {
            return await context.Orders.Include(o => o.Address)
                                       .Include(o => o.Payment)
                                       .Include(o => o.Schedule)
                                       .Include(o => o.Customer)
                                       .ThenInclude(c => c.Account)
                                       .Where(o => o.ScheduleId == scheduleId)
                                       .OrderByDescending(o => o.OrderDate)
                                       .Skip(offset)
                                       .Take(pageSize)
                                       .ToListAsync();
        }
    }
}
