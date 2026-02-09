using CleanAgricultureProductBE.DTOs.Order;

namespace CleanAgricultureProductBE.Services.Order
{
    public interface IOrderService
    {
        public Task<string> PlaceOrder(string accountEmail, OrderRequestDto request);
    }
}
