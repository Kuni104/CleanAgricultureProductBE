
using CleanAgricultureProductBE.Repositories.OrderDetail;

namespace CleanAgricultureProductBE.Services.OrderDetail
{
    public class OrderDetailService(IOrderDetailRepository orderDetailRepository) : IOrderDetailService
    {
        public Task AddOrderDetails(List<Models.OrderDetail> orderDetails)
        {
            throw new NotImplementedException();
        }
    }
}
