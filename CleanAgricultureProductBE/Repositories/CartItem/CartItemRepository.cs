using CleanAgricultureProductBE.Data;

namespace CleanAgricultureProductBE.Repositories.CartItem
{
    public class CartItemRepository(AppDbContext context) : ICartItemRepository
    {
        public async Task AddCartItem(Models.CartItem cartItem)
        {
            context.CartItems.Add(cartItem);
            await context.SaveChangesAsync();
        }

        //public Task<string> GetCartItemByCartIdAndProductId(Guid cartId, string productId)
        //{
            
        //}
    }
}
