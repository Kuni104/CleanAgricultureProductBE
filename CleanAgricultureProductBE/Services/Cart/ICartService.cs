using CleanAgricultureProductBE.DTOs.Cart;

namespace CleanAgricultureProductBE.Services.Cart
{
    public interface ICartService
    {
        public Task<string> AddToCart(string userId, AddToCartRequestDto request);
    }
}
