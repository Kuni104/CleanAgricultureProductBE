using CleanAgricultureProductBE.Data;

namespace CleanAgricultureProductBE.Repositories.OrderDetail
{
    public class OrderDetailRepository(AppDbContext context) : IOrderDetailRepository
    {
        public async Task AddOrderDetails(List<Models.OrderDetail> orderDetails)
        {
            context.OrderDetails.AddRange(orderDetails);
            await context.SaveChangesAsync();
        }
    }
}
