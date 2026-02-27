
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Order;
using CleanAgricultureProductBE.DTOs.OrderDetail;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Repositories.Cart;
using CleanAgricultureProductBE.Repositories.CartItem;
using CleanAgricultureProductBE.Repositories.DeliveryFee;
using CleanAgricultureProductBE.Repositories.Order;
using CleanAgricultureProductBE.Repositories.OrderDetail;
using CleanAgricultureProductBE.Repositories.Payment;
using CleanAgricultureProductBE.Services.Cart;
using CleanAgricultureProductBE.Services.DeliveryFee;

namespace CleanAgricultureProductBE.Services.Order
{
    public class OrderService(IAccountRepository accountRepository, IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository, ICartRepository cartRepository,IDeliveryFeeRepository deliveryFeeRepository, IPaymentRepository paymentRepository ) : IOrderService
    {
        //UTC+7 timezone
        private readonly TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        //Place order
        public async Task<OrderResponseDto> PlaceOrder(string accountEmail, OrderRequestDto request)
        {
            //var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            var cart = await GetCartByAccoutEmail(accountEmail);

            var cartItems = await cartRepository.GetCartItemsByCartId(cart!.CartId);
            var deliveryFee = await deliveryFeeRepository.GetDeliveryFeeById(request.DeliveryFeeId);

            decimal totalCartPrice = await cartRepository.TotalPriceOfCartByCartId(cart.CartId);
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
            await cartRepository.DeleteAllCartItems(cart.CartId);

            order = await orderRepository.GetOrderByOrderId(order.OrderId);

            var orderReponse = new OrderResponseDto
            {
                OrderId = order!.OrderId,
                CustomerName = order.Address.RecipientName,
                Address = order.Address.AddressDetail,
                //Payment = (Guid)order.PaymentId,
                Schedule = TimeZoneInfo.ConvertTimeFromUtc(order.Schedule.ScheduledDate, timeZone),
                TotalPrice = totalOrderPrice,
                OrderDate = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, timeZone),
                OrderStatus = order.OrderStatus,            
            };

            return orderReponse;
        }

        //Get all orders
        public async Task<ResponseDtoWithPagination<List<OrderResponseDto>>> GetAllOrders(string accountEmail, int? page, int? size, string? keyword)
        {
            bool isPagination = false;
            int offset = 0;
            int pageSize = 0;
            if (page != null && size != null)
            {
                pageSize = (int)size;
                offset = (int)((page - 1) * size);
                isPagination = true;
            }

            var account = await accountRepository.GetByEmailAsync(accountEmail);
            var customerId = account!.UserProfile.UserProfileId;

            var orders = await orderRepository.GetOrdersByCustomerId(customerId);

            int totalItems = orders.Count;

            if (isPagination) {
                orders = await orderRepository.GetOrdersByCustomerIdWithPagination(customerId, offset, pageSize);
            }

            var orderResponseList = new List<OrderResponseDto>();
            foreach (var order in orders)
            {
                orderResponseList.Add(new OrderResponseDto
                {
                    OrderId = order.OrderId,
                    CustomerName = order.Address.RecipientName,
                    Address = order.Address.AddressDetail,
                    //Payment = (Guid)order.PaymentId,
                    Schedule = TimeZoneInfo.ConvertTimeFromUtc(order.Schedule.ScheduledDate, timeZone),
                    TotalPrice = order.Payment.TotalAmount,
                    OrderDate = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, timeZone),
                    OrderStatus = order.OrderStatus
                });
            }

            var result = new ResponseDtoWithPagination<List<OrderResponseDto>>
            {
                ResultObject = orderResponseList
            };

            if (isPagination)
            {
                int totalPage = totalItems / pageSize + (totalItems % pageSize == 0 ? 1 : 0);
                if (totalItems < pageSize || page <= size)
                {
                    totalPage = 1;
                }

                result.Pagination = new Pagination
                {
                    PageNumber = (int)page!,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPage
                };
            }

            return result;
        }

        //Get order detail
        public async Task<ResponseDtoWithPagination<OrderDetailListResponseDto>> GetOrderDetails(string accountEmail, Guid orderId, int? page, int? size, string? keyword)
        {
            var order = await orderRepository.GetOrderByOrderId(orderId);
            if(order!.Customer.Account.Email != accountEmail)
            {
                return null!;
            }

            bool isPagination = false;
            int offset = 0;
            int pageSize = 0;
            if (page != null && size != null)
            {
                pageSize = (int)size;
                offset = (int)((page - 1) * size);
                isPagination = true;
            }

            var orderDetailList = await orderDetailRepository.GetOrderDetailsByOrderId(orderId);

            int totalItems = orderDetailList.Count;

            if (isPagination)
            {
                orderDetailList = await orderDetailRepository.GetOrderDetailsByOrderIdWithPagination(orderId, offset, pageSize, keyword);
            }

            var orderDetailResponseList = new List<OrderDetailResponseDto>();
            foreach (var orderDetail in orderDetailList)
            {
                orderDetailResponseList.Add(new OrderDetailResponseDto
                {
                    OrderDetailId = orderDetail.OrderDetailId,
                    ProductId = orderDetail.ProductId,
                    ProductName = orderDetail.Product.Name,
                    Quantity = orderDetail.Quantity,
                    TotalPrice = orderDetail.TotalPrice,
                    CreatedAt = TimeZoneInfo.ConvertTimeFromUtc(orderDetail.CreatedAt, timeZone),
                    ExpiryDate = TimeZoneInfo.ConvertTimeFromUtc(orderDetail.ExpiryDate, timeZone)
                });
            }

            var orderDetailsResponse = new OrderDetailListResponseDto
            {
                OrderId = order.OrderId,
                OrderDetails = orderDetailResponseList,
                TotalPrice = order.Payment.TotalAmount
            };

            var result = new ResponseDtoWithPagination<OrderDetailListResponseDto>
            {
                ResultObject = orderDetailsResponse
            };

            if (isPagination)
            {
                int totalPage = totalItems / pageSize + (totalItems % pageSize == 0 ? 1 : 0);
                if (totalItems < pageSize || page <= size)
                {
                    totalPage = 1;
                }

                result.Pagination = new Pagination
                {
                    PageNumber = (int)page!,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPage
                };
            }

            return result;
        }

        private async Task<Models.Cart> GetCartByAccoutEmail(string accountEmail)
        {
            var account = await accountRepository.GetByEmailAsync(accountEmail);
            var customerId = account!.UserProfile.UserProfileId;
            var cart = await cartRepository.GetCartByCustomerId(customerId);

            return cart!;
        }
    }
}
