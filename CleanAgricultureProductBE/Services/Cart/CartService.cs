using CleanAgricultureProductBE.DTOs.Cart;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Repositories.Cart;
using CleanAgricultureProductBE.Repositories.CartItem;

namespace CleanAgricultureProductBE.Services.Cart
{
    public class CartService(IAccountRepository accountRepository, IProductRepository productRepository, ICartRepository cartRepository, ICartItemRepository cartItemRepository) : ICartService
    {

        public async Task<string> AddToCart(string accountId, AddToCartRequestDto request)
        {
            //var account = await accountRepository.GetAccountByIdAsync(accountId);

            //var customerId = account!.UserProfile.UserProfileId;

            //var cart = await cartRepository.GetCartByCustomerId(customerId);

            //var product = await productRepository.GetByIdAsync(request.ProductId);

            return "Item added to cart successfully.";
        }
    }
}
