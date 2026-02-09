namespace CleanAgricultureProductBE.Services.OrderDetail
{
    public interface IOrderDetailService
    {
        public Task AddOrderDetails(List<Models.OrderDetail> orderDetails);
    }
}
