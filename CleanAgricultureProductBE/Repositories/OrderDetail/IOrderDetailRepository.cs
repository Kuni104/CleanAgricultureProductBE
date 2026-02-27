namespace CleanAgricultureProductBE.Repositories.OrderDetail
{
    public interface IOrderDetailRepository
    {
        public Task AddOrderDetails(List<Models.OrderDetail> orderDetails);

        public Task<List<Models.OrderDetail>> GetOrderDetailsByOrderId(Guid orderId);
        public Task<List<Models.OrderDetail>> GetOrderDetailsByOrderIdWithPagination(Guid orderId, int offset, int pageSize, string? keyword);
    }
}
