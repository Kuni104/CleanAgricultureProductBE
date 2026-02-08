using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Cart;
using CleanAgricultureProductBE.DTOs.CartItem;
using CleanAgricultureProductBE.DTOs.Response;

namespace CleanAgricultureProductBE.Services.Cart
{
    public interface ICartService
    {
        public Task<AddToCartResponseDto> AddToCart(string userId, Guid productId, CartRequestDto request);
        public Task<CartItemResponseWithPaginationDto> GetCartItem(string accountEmail, int? page, int? size, string? keyword);
        public Task<UpdateCartResponseDto> UpdateCartItemQuantity(string accountEmail, Guid productId, CartRequestDto request);
        public Task<ResultStatusWithData<decimal>> DeleteCartItem(string accountEmail, Guid productId);
        public Task<string> DeleteAllCartItems(string accountEmail);
    }
}
