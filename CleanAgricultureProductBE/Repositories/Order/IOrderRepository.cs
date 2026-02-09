using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories.Order
{
    public interface IOrderRepository
    {
        public Task AddOrder(Models.Order order);
    }
}
