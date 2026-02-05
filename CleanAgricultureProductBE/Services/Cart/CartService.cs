using CleanAgricultureProductBE.DTOs.Cart;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories.Cart;
using CleanAgricultureProductBE.Repositories.CartItem;

namespace CleanAgricultureProductBE.Services.Cart
{
    public class CartService(IAccountRepository accountRepository, IProductRepository productRepository, ICartRepository cartRepository, ICartItemRepository cartItemRepository) : ICartService
    {

        public async Task<AddToCartResponseDto> AddToCart(string accountEmail, AddToCartRequestDto request)
        {
            var account = await accountRepository.GetByEmailAsync(accountEmail);
            var customerId = account!.UserProfile.UserProfileId;

            var cart = await cartRepository.GetCartByCustomerId(customerId);
            var product = await productRepository.GetByIdAsync(request.ProductId);
            //var cartItems = await cartItemRepository.GetCartItemByCartIdAndProductId

            var cartItem = new CartItem
            {
                CartItemId = Guid.NewGuid(),
                CartId = cart!.CartId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            await cartItemRepository.AddCartItem(cartItem);

            return new AddToCartResponseDto
            {
                CartItemId = cartItem.CartItemId,
                CartId = cart.CartId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                TotalPrice = product!.Price * request.Quantity
            };
        }
    }
}
