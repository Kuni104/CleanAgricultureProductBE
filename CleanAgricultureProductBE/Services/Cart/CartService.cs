using CleanAgricultureProductBE.DTOs.Cart;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories.Cart;
using CleanAgricultureProductBE.Repositories.CartItem;
using CleanAgricultureProductBE.DTOs.CartItem;

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

            var cartItem = await cartItemRepository.GetCartItemByCartIdAndProductId(cart!.CartId, product!.ProductId);

            if (cartItem is null)
            {
                cartItem = new CartItem
                {
                    CartItemId = Guid.NewGuid(),
                    CartId = cart!.CartId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    CreatedAt = DateTime.UtcNow
                };

                await cartItemRepository.AddCartItem(cartItem);
            }
            else
            {
                cartItem.Quantity += request.Quantity;
                await cartItemRepository.UpdateCartItem(cartItem);
            }

            return new AddToCartResponseDto
            {
                CartItemId = cartItem.CartItemId,
                CartId = cart.CartId,
                ProductId = request.ProductId,
                Quantity = cartItem.Quantity,
                TotalPrice = product!.Price * cartItem.Quantity
            };
        }

        public async Task<List<GetCartItemDto>> GetCartItem(string accountEmail)
        {
            var account = await accountRepository.GetByEmailAsync(accountEmail);
            var customerId = account!.UserProfile.UserProfileId;
            var cart = await cartRepository.GetCartByCustomerId(customerId);

            var cartItemList = await cartItemRepository.GetCartItemsByCartId(cart!.CartId);

            List<GetCartItemDto> cartItems = new List<GetCartItemDto>();
            foreach (var item in cartItemList)
            {
                var product = await productRepository.GetByIdAsync(item.ProductId);
                cartItems.Add(new GetCartItemDto
                {
                    ProductId = product!.ProductId,
                    ProductName = product!.Name,
                    Price = product.Price,
                    Quantity = item.Quantity,
                    TotalPrice = product.Price * item.Quantity
                });
            }

            return cartItems;
        }
    }
}
