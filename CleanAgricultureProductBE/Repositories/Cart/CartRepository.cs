using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.Cart
{
    public class CartRepository(AppDbContext context) : ICartRepository
    {
        //Cart
        public async Task<Models.Cart?> GetCartByCustomerId(Guid customerId)
        {
            return await context.Carts
                                 .Where(c => c.CustomerId == customerId)
                                 .FirstOrDefaultAsync();
        }

        //Cart Items
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

        public async Task<List<Models.CartItem>> GetCartItemsByCartIdIncludeProduct(Guid cartId)
        {
            return await context.CartItems.Include(ci => ci.Product)
                                          .Where(ci => ci.CartId == cartId)
                                          .ToListAsync();
        }

        public async Task<Models.CartItem?> GetCartItemByCartIdAndProductId(Guid cartId, Guid productId)
        {
            return await context.CartItems.Where(ci => ci.CartId == cartId && ci.ProductId == productId)
                                          .FirstOrDefaultAsync();
        }

        public async Task<Models.CartItem?> GetCartItemByCartIdAndProductIdIncludeProduct(Guid cartId, Guid productId)
        {
            return await context.CartItems.Include(ci => ci.Product)
                                          .Where(ci => ci.CartId == cartId && ci.ProductId == productId)
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

        public async Task DeleteAllCartItems(Guid cartId)
        {
            await context.CartItems.Where(ci => ci.CartId == cartId)
                             .ExecuteDeleteAsync();
        }

        public async Task<decimal> TotalPriceOfCartByCartId(Guid cartId)
        {
            return await context.CartItems.Where(ci => ci.CartId == cartId)
                                          .SumAsync(ci => ci.TotalPrice);
        }

    }
}
