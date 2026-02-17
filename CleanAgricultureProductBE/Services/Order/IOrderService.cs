using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Order;

namespace CleanAgricultureProductBE.Services.Order
{
    public interface IOrderService
    {
        public Task<OrderResponseDto> PlaceOrder(string accountEmail, OrderRequestDto request);
        public Task<ResponseDtoWithPagination<List<OrderResponseDto>>> GetAllOrders(string accountEmail, int? size, int? page, string? keyword);
    }
}
