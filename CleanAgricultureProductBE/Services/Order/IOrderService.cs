using CleanAgricultureProductBE.DTOs.Order;

namespace CleanAgricultureProductBE.Services.Order
{
    public interface IOrderService
    {
        public Task<OrderResponseDto> PlaceOrder(string accountEmail, OrderRequestDto request);
    }
}
