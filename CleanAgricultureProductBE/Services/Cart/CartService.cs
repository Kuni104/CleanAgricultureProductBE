using CleanAgricultureProductBE.DTOs.Cart;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories.Cart;
using CleanAgricultureProductBE.Repositories.CartItem;

namespace CleanAgricultureProductBE.Services.Cart
{
    public class CartService(IAccountRepository accountRepository, ICartRepository cartRepository, ICartItemRepository cartItemRepository) : ICartService
    {

        public async Task<string> AddToCart(string accountId, AddToCartRequestDto request)
        {
            var account = await accountRepository.GetAccountById(accountId);
            var customerId = account!.UserProfile.UserProfileId;



            var cart = await cartRepository.GetCartByCustomerId(customerId);

            var cartItem = new CartItem
            {
                CartItemId = Guid.NewGuid(),
                CartId = cart!.CartId,
                ProductId = Guid.Parse(request.ProductId!),
                Quantity = request.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            await cartItemRepository.AddCartItem(cartItem);

            return "Item added to cart successfully.";
        }
    }
}
