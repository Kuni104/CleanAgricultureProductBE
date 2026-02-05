using CleanAgricultureProductBE.Models;

namespace CleanAgricultureProductBE.Repositories.Cart
{
    public interface ICartRepository
    {
        public Task<Models.Cart> GetCartByCustomerId(Guid customerId);
    }
}
