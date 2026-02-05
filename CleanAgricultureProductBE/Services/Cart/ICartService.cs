using CleanAgricultureProductBE.DTOs.Cart;

namespace CleanAgricultureProductBE.Services.Cart
{
    public interface ICartService
    {
        public Task<AddToCartResponseDto> AddToCart(string userId, AddToCartRequestDto request);
    }
}
