using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Order;
using CleanAgricultureProductBE.DTOs.OrderDetail;

namespace CleanAgricultureProductBE.Services.Order
{
    public interface IOrderService
    {
        public Task<OrderResponseDto> PlaceOrder(string accountEmail, OrderRequestDto request);
        public Task<ResponseDtoWithPagination<List<OrderResponseDto>>> GetAllOrders(string accountEmail, int? size, int? page, string? keyword);
        public Task<ResponseDtoWithPagination<OrderDetailListResponseDto>> GetOrderDetails(string accountEmail, Guid orderId, int? page, int? size, string? keyword);
    }
}
