using CleanAgricultureProductBE.DTOs.Cart;
using CleanAgricultureProductBE.DTOs.CartItem;
using CleanAgricultureProductBE.DTOs.Response;

namespace CleanAgricultureProductBE.Services.Cart
{
    public interface ICartService
    {
        public Task<AddToCartResponseDto> AddToCart(string userId, AddToCartRequestDto request);
        public Task<CartItemWithPaginationDto> GetCartItem(string accountEmail, int? page, int? size, string? keyword);
        public Task<GetCartItemReponseDto> UpdateCartItem(string accountEmail, UpdateCartItemRequestDto request);
    }
}
