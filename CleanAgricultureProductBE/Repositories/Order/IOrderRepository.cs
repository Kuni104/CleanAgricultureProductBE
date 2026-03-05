using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories.Order
{
    public interface IOrderRepository
    {
        public Task<List<Models.Order>> GetAllOrders();
        public Task<List<Models.Order>> GetAllOrdersInSchedule(Guid scheduleId);
        public Task<List<Models.Order>> GetAllOrdersInScheduleWithPagination(Guid scheduleId, int offset, int pageSize);
        public Task<List<Models.Order>> GetAllOrdersWithPagination(int offset, int pageSize);
        public Task<Models.Order?> GetOrderByOrderId(Guid orderId);
        public Task AddOrder(Models.Order order);
        public Task UpdateOrder(Models.Order order);
        public Task<Models.Order> GetOrderById(Guid orderId);
        Task UpdateAsync(Models.Order order);
        public Task<List<Models.Order>> GetOrdersByCustomerId(Guid customerId);
        public Task<List<Models.Order>> GetOrdersByCustomerIdWithPagination(Guid customerId, int offset, int pageSize);
        public Task TestUpdateOrderWithPayment(Models.Order order);
    }
}
