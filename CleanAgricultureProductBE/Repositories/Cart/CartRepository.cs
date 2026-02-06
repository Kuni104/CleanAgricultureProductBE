using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Repositories.Cart
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Models.Cart?> GetCartByCustomerId(Guid customerId)
        {
            return await _context.Carts
                                 .Where(c => c.CustomerId == customerId)
                                 .FirstOrDefaultAsync();
        }

    }
}
