
using CleanAgricultureProductBE.Data;

namespace CleanAgricultureProductBE.Repositories.Order
{
    public class OrderRepository(AppDbContext context) : IOrderRepository
    {
        public async Task AddOrder(Models.Order order)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();
        }
    }
}
