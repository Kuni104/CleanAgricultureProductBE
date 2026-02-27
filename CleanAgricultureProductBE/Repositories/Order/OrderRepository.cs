using CleanAgricultureProductBE.Data;
using System;
using System.Threading.Tasks;

namespace CleanAgricultureProductBE.Repositories.Order
{
    public class OrderRepository(AppDbContext context) : IOrderRepository
    {
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
    }
}
