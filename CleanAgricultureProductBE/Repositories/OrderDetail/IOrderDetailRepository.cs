namespace CleanAgricultureProductBE.Repositories.OrderDetail
{
    public interface IOrderDetailRepository
    {
        public Task AddOrderDetails(List<Models.OrderDetail> orderDetails);
    }
}
