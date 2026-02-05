using CleanAgricultureProductBE.DTOs.Cart;
using CleanAgricultureProductBE.Repositories.Cart;

namespace CleanAgricultureProductBE.Services.Cart
{
    public class CartService(ICartRepository cartRepository) : ICartService
    {
        public async Task<string> AddToCart(AddToCartRequestDto request)
        {
            return "Item added to cart successfully.";
        }
    }
}
