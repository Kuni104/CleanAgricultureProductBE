
using CleanAgricultureProductBE.DTOs.Order;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Repositories.CartItem;
using CleanAgricultureProductBE.Services.Cart;

namespace CleanAgricultureProductBE.Services.Order
{
    public class OrderService(IAccountRepository accountRepository, ICartService cartService, ICartItemRepository cartItemRepository) : IOrderService
    {
        public async Task<string> PlaceOrder(string accountEmail, OrderRequestDto request)
        {
            var cart = await cartService.GetCartByAccoutEmail(accountEmail);

            var cartItems = await cartItemRepository.GetCartItemsByCartId(cart!.CartId);

            decimal totalCartPrice = await cartService.TotalPriceOfCartByCartId(cart.CartId);

            List<Models.OrderDetail> orderDetails = new List<Models.OrderDetail>();

            var order = new Models.Order
            {
                OrderId = Guid.NewGuid(),
                CustomerId = cart.CustomerId,
                AddressId = request.AddressId,
                DeliveryFeeId = request.DeliveryFeeId,
            };


            foreach (var item in cartItems)
            {
                orderDetails.Add(new Models.OrderDetail
                {
                    OrderDetailId = Guid.NewGuid(),
                }
                    );
            }


            return null!;
        }
    }
}
