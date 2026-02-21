using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories.Order
{
    public interface IOrderRepository
    {
        public Task<Models.Order?> GetOrderByOrderId(Guid orderId);
        public Task AddOrder(Models.Order order);
        public Task<List<Models.Order>> GetOrdersByCustomerId(Guid customerId);
        public Task<List<Models.Order>> GetOrdersByCustomerIdWithPagination(Guid customerId, int offset, int pageSize);
    }
}
