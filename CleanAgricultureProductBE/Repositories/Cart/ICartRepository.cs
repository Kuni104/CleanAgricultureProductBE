using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories.Cart
{
    public interface ICartRepository
    {
        //Cart
        public Task<Models.Cart?> GetCartByCustomerId(Guid customerId);
        //Cart Items
        public Task AddCartItem(Models.CartItem cartItem);
        public Task<List<Models.CartItem>> GetCartItemsByCartId(Guid cartId);
        public Task<List<Models.CartItem>> GetCartItemsByCartIdIncludeProduct(Guid cartId);
        public Task<List<Models.CartItem>> GetCartItemsByCartIdWithPagination(Guid cartId, int offset, int pageSize);
        public Task<bool> CheckCartItemByCartIdAndProductId(Guid cartId, Guid productId);
        public Task<Models.CartItem?> GetCartItemByCartIdAndProductId(Guid cartId, Guid productId);
        public Task<Models.CartItem?> GetCartItemByCartIdAndProductIdIncludeProduct(Guid cartId, Guid productId);
        public Task UpdateCartItem(Models.CartItem cartItem);
        public Task DeleteCartItem(Models.CartItem cartItem);
        public Task DeleteAllCartItems(Guid cartId);
        public Task<decimal> TotalPriceOfCartByCartId(Guid cartId);
    }
}
