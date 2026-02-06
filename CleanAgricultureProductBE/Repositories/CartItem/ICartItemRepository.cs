namespace CleanAgricultureProductBE.Repositories.CartItem
{
    public interface ICartItemRepository
    {
        public Task AddCartItem(Models.CartItem cartItem);

        public Task<List<Models.CartItem>> GetCartItemsByCartId(Guid cartId);

        public Task<bool> CheckCartItemByCartIdAndProductId(Guid cartId, Guid productId);

        public Task<Models.CartItem?> GetCartItemByCartIdAndProductId(Guid cartId, Guid productId);

        public Task UpdateCartItem(Models.CartItem cartItem);
    }
}
