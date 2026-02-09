
using CleanAgricultureProductBE.DTOs.Order;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories.CartItem;
using CleanAgricultureProductBE.Repositories.DeliveryFee;
using CleanAgricultureProductBE.Repositories.Order;
using CleanAgricultureProductBE.Repositories.OrderDetail;
using CleanAgricultureProductBE.Repositories.Payment;
using CleanAgricultureProductBE.Services.Cart;

namespace CleanAgricultureProductBE.Services.Order
{
    public class OrderService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, ICartService cartService, ICartItemRepository cartItemRepository, IDeliveryFeeRepository deliveryFeeRepository, IPaymentRepository paymentRepository ) : IOrderService
    {
        public async Task<OrderResponseDto> PlaceOrder(string accountEmail, OrderRequestDto request)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            var cart = await cartService.GetCartByAccoutEmail(accountEmail);

            var cartItems = await cartItemRepository.GetCartItemsByCartId(cart!.CartId);
            var deliveryFee = await deliveryFeeRepository.GetDeliveryFeeById(request.DeliveryFeeId);

            decimal totalCartPrice = await cartService.TotalPriceOfCartByCartId(cart.CartId);
            var totalOrderPrice = totalCartPrice + deliveryFee!.FeeAmount;

            var payment = new Models.Payment();

            if (request.PaymentMethodId == 1)
            {
                payment.PaymentId = Guid.NewGuid();
                payment.PaymentMethodId = request.PaymentMethodId;
                payment.PaymentStatus = "Waiting";
                payment.TotalAmount = totalOrderPrice;
                payment.CreatedAt = DateTime.UtcNow;
            }

            await paymentRepository.AddPayment(payment);

            var order = new Models.Order
            {
                OrderId = Guid.NewGuid(),
                CustomerId = cart.CustomerId,
                AddressId = request.AddressId,
                DeliveryFeeId = request.DeliveryFeeId,
                ScheduleId = request.ScheduleId,
                PaymentId = payment.PaymentId,
                OrderDate = DateTime.UtcNow,
                OrderStatus = "Waiting For Deliver"
            };

            await orderRepository.AddOrder(order);

            List<Models.OrderDetail> orderDetails = new List<Models.OrderDetail>();

            foreach (var item in cartItems)
            {
                orderDetails.Add(new Models.OrderDetail
                {
                    OrderDetailId = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                    CreatedAt = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(30),
                });
            }

            await orderDetailRepository.AddOrderDetails(orderDetails);
            await cartItemRepository.DeleteAllCartItems(cart.CartId);

            var orderReponse = new OrderResponseDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                AddressId = order.AddressId,
                PaymentId = (Guid)order.PaymentId,
                ScheduleId = order.ScheduleId,
                TotalPrice = totalOrderPrice,
                OrderDate = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, timeZone),
                OrderStatus = order.OrderStatus,            
            };

            return orderReponse;
        }
    }
}
