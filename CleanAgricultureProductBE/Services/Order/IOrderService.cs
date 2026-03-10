using Azure.Core;
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Order;
using CleanAgricultureProductBE.DTOs.OrderDetail;
using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Services.Order
{
    public interface IOrderService
    {
        public Task<ResultStatusWithData<PlaceOrderResponseDto>> PlaceOrder(string accountEmail, OrderRequestDto request);
        public Task<ResponseDtoWithPagination<List<OrderResponseDto>>> GetAllOrders(string accountEmail, int? size, int? page, string? keyword);
        public Task<ResponseDtoWithPagination<OrderDetailListResponseDto>> GetOrderDetails(string accountEmail, Guid orderId, int? page, int? size, string? keyword);
        public Task<ResponseDtoWithPagination<OrderDetailListResponseDto>> GetOrderDetailsAdmin(Guid orderId, int? page, int? size, string? keyword);
        public Task<ResponseDtoWithPagination<List<OrderResponseDto>>> GetAllOrdersAdmin(int? page, int? size, string? keyword);
        public Task<ResponseDtoWithPagination<List<OrderResponseDto>>> GetAllOrdersInSchedule(Guid scheduleId, int? page, int? size, string? keyword);
        public Task<OrderResponseDto> UpdateOrderStatus(Guid orderId, UpdateOrderStatusRequestDto request);
        public Task<ResultStatusWithData<OrderResponseDto>> UpdateOrderAddress(string accountEmail, Guid orderId, UpdateOrderAddressRequestDto request);
    }
}
