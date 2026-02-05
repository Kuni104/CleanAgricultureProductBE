namespace CleanAgricultureProductBE.Repositories.CartItem
{
    public interface ICartItemRepository
    {
        public Task AddCartItem(Models.CartItem cartItem);
    }
}
