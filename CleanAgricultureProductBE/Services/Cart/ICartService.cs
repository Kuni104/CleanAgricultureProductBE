using CleanAgricultureProductBE.DTOs.Cart;
using CleanAgricultureProductBE.DTOs.CartItem;

namespace CleanAgricultureProductBE.Services.Cart
{
    public interface ICartService
    {
        public Task<AddToCartResponseDto> AddToCart(string userId, AddToCartRequestDto request);
        public Task<List<GetCartItemDto>> GetCartItem(string accountEmail);
    }
}
