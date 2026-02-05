namespace CleanAgricultureProductBE.Repositories.CartItem
{
    public interface ICartItemRepository
    {
        public Task AddCartItem(Models.CartItem cartItem);

        //public Task<string> GetCartItemByCartIdAndProductId(Guid cartId, string productId);
    }
}
