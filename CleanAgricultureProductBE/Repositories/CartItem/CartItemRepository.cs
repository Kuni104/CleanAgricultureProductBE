using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.CartItem
{
    public class CartItemRepository(AppDbContext context) : ICartItemRepository
    {
        public async Task AddCartItem(Models.CartItem cartItem)
        {
            context.CartItems.Add(cartItem);
            await context.SaveChangesAsync();
        }

        public async Task<List<Models.CartItem>> GetCartItemsByCartId(Guid cartId)
        {
            return await context.CartItems.Where(ci => ci.CartId == cartId)
                                          .ToListAsync();
        }

        public async Task<Models.CartItem?> GetCartItemByCartIdAndProductId(Guid cartId, Guid productId)
        {
            return await context.CartItems.Where(ci => ci.CartId == cartId && ci.ProductId == productId)
                                          .FirstOrDefaultAsync();
        }

        public async Task<bool> CheckCartItemByCartIdAndProductId(Guid cartId, Guid productId)
        {
            return await context.CartItems.AnyAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        public async Task UpdateCartItem(Models.CartItem cartItem)
        {
            context.CartItems.Update(cartItem);
            await context.SaveChangesAsync();
        }

        public async Task<List<Models.CartItem>> GetCartItemsByCartIdWithPagination(Guid cartId, int offset, int pageSize)
        {
            return await context.CartItems.Where(ci => ci.CartId == cartId)
                                          .OrderByDescending(ci => ci.CreatedAt)
                                          .Skip(offset)
                                          .Take(pageSize)
                                          .ToListAsync();
        }

        public async Task DeleteCartItem(Models.CartItem cartItem)
        {
            context.CartItems.Remove(cartItem);
            await context.SaveChangesAsync();
        }

    }
}
