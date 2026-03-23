using CleanAgricultureProductBE.Repositories.Cart;
using CleanAgricultureProductBE.Repositories.Order;
using CleanAgricultureProductBE.Repositories.Payment;
using CleanAgricultureProductBE.Repositories.Product;

namespace CleanAgricultureProductBE.Services.Payment
{
    public class PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICartRepository cartRepository) : IPaymentService
    {
        public Task CreatePayment()
        {
            throw new NotImplementedException();
        }

        public async Task HandlePaymentResult(string orderId, string? transactionCode, bool isSuccess)
        {
            if (!Guid.TryParse(orderId, out var parsedOrderId))
            {
                throw new ArgumentException("Order ID khong hop le.", nameof(orderId));
            }

            var order = await orderRepository.GetOrderByOrderId(parsedOrderId);
            if (order == null || order.Payment == null)
            {
                throw new Exception($"Order with ID {orderId} not found.");
            }

            var payment = await paymentRepository.GetPaymentById(order.Payment.PaymentId);
            if (payment == null)
            {
                throw new Exception($"Payment for order {orderId} not found.");
            }

            if (isSuccess)
            {
                if (string.Equals(payment.PaymentStatus, "Failed", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                payment.PaymentStatus = "Paid";
                if (!string.IsNullOrWhiteSpace(transactionCode))
                {
                    payment.TransactionCode = transactionCode;
                }

                order.OrderStatus = "Pending";

                await paymentRepository.UpdatePayment(payment);
                await orderRepository.UpdateOrder(order);
                return;
            }

            if (string.Equals(payment.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(payment.PaymentStatus, "Failed", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            payment.PaymentStatus = "Failed";
            if (!string.IsNullOrWhiteSpace(transactionCode))
            {
                payment.TransactionCode = transactionCode;
            }

            order.OrderStatus = "Cancelled";

            await RestoreInventoryAsync(order);
            await RestoreCartAsync(order);

            await paymentRepository.UpdatePayment(payment);
            await orderRepository.UpdateOrder(order);
        }

        private async Task RestoreInventoryAsync(Models.Order order)
        {
            foreach (var item in order.OrderDetails)
            {
                item.Product.Stock += item.Quantity;
                await productRepository.UpdateAsync(item.Product);
            }
        }

        private async Task RestoreCartAsync(Models.Order order)
        {
            var cart = await cartRepository.GetCartByCustomerId(order.CustomerId);
            if (cart == null)
            {
                cart = new Models.Cart
                {
                    CartId = Guid.NewGuid(),
                    CustomerId = order.CustomerId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await cartRepository.CreateCart(cart);
            }

            foreach (var item in order.OrderDetails)
            {
                var existingCartItem = await cartRepository.GetCartItemByCartIdAndProductId(cart.CartId, item.ProductId);
                if (existingCartItem == null)
                {
                    await cartRepository.AddCartItem(new Models.CartItem
                    {
                        CartItemId = Guid.NewGuid(),
                        CartId = cart.CartId,
                        ProductId = item.ProductId,
                        UnitPrice = item.TotalPrice / item.Quantity,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice,
                        CreatedAt = DateTime.UtcNow,
                    });
                    continue;
                }

                existingCartItem.Quantity += item.Quantity;
                existingCartItem.TotalPrice = existingCartItem.UnitPrice * existingCartItem.Quantity;
                await cartRepository.UpdateCartItem(existingCartItem);
            }
        }
    }
}
