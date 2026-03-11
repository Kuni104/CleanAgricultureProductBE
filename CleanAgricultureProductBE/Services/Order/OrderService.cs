
using CleanAgricultureProductBE.DTOs.ApiResponse;
using CleanAgricultureProductBE.DTOs.Order;
using CleanAgricultureProductBE.DTOs.OrderDetail;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Repositories.Cart;
using CleanAgricultureProductBE.Repositories.CartItem;
using CleanAgricultureProductBE.Repositories.CycleSchedule;
using CleanAgricultureProductBE.Repositories.DeliveryFee;
using CleanAgricultureProductBE.Repositories.DSchedule;
using CleanAgricultureProductBE.Repositories.Order;
using CleanAgricultureProductBE.Repositories.OrderDetail;
using CleanAgricultureProductBE.Repositories.Payment;
using CleanAgricultureProductBE.Repositories.Product;
using CleanAgricultureProductBE.Services.Cart;
using CleanAgricultureProductBE.Services.DeliveryFee;
using CleanAgricultureProductBE.Services.VnPay;

namespace CleanAgricultureProductBE.Services.Order
{
    public class OrderService(IAccountRepository accountRepository, IOrderRepository orderRepository, IScheduleRepository scheduleRepository , ICycleScheduleRepository cycleScheduleRepository, IProductRepository productRepository, IOrderDetailRepository orderDetailRepository, ICartRepository cartRepository,IDeliveryFeeRepository deliveryFeeRepository, IPaymentRepository paymentRepository, IVnPayService vnPayService ) : IOrderService
    {
        //UTC+7 timezone
        private readonly TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        //Place order
        public async Task<ResultStatusWithData<PlaceOrderResponseDto>> PlaceOrder(string accountEmail, OrderRequestDto request)
        {
            //var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var deliveryFee = await deliveryFeeRepository.GetDeliveryFeeById(request.DeliveryFeeId);
            if (deliveryFee == null)
            {
                return new ResultStatusWithData<PlaceOrderResponseDto>
                {
                    Status = "Delivery Fee 404",
                    Data = null
                };
            }

            var account = await accountRepository.GetByEmailAsync(accountEmail);
            var address = account!.UserProfile.Addresses.FirstOrDefault(a => a.AddressId == request.AddressId);
            if (address == null)
            {
                return new ResultStatusWithData<PlaceOrderResponseDto>
                {
                    Status = "Address 404",
                    Data = null
                };
            }

            var isCycleSchedule = false;

            if (request.IsCycleSchedule == true)
            {
                isCycleSchedule = true;

                if (request.IsMonthly == false && (request.DayCycle == null || request.DayCycle <= 0))
                {
                    return new ResultStatusWithData<PlaceOrderResponseDto>
                    {
                        Status = "Day Cycle Error",
                        Data = null
                    };
                }
            }

            var cart = await GetCartByAccoutEmail(accountEmail);

            var cartItems = await cartRepository.GetCartItemsByCartId(cart!.CartId);

            if (cartItems == null || cartItems.Count == 0)
            {
                return null!;
            }

            decimal totalCartPrice = await cartRepository.TotalPriceOfCartByCartId(cart.CartId);
            var totalOrderPrice = totalCartPrice + deliveryFee!.FeeAmount;

            var payment = new Models.Payment();

            payment.PaymentId = Guid.NewGuid();
            payment.PaymentMethodId = request.PaymentMethodId;
            payment.PaymentStatus = "Pending";
            payment.TotalAmount = totalOrderPrice;
            payment.CreatedAt = DateTime.UtcNow;

            await paymentRepository.AddPayment(payment);

            var order = new Models.Order
            {
                OrderId = Guid.NewGuid(),
                CustomerId = cart.CustomerId,
                AddressId = request.AddressId,
                DeliveryFeeId = request.DeliveryFeeId,
                PaymentId = payment.PaymentId,
                OrderDate = DateTime.UtcNow,
                OrderStatus = "Pending"
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

            foreach (var items in orderDetails ) 
            {
                var product = items.Product;
                product.Stock = product.Stock - items.Quantity;
                await productRepository.UpdateAsync(product);
            }

            order = await orderRepository.GetOrderByOrderId(order.OrderId);

            string paymentUrl = string.Empty;

            if (request.PaymentMethodId == 2) {
                paymentUrl = vnPayService.CreatePaymentUrl(new VNPAY.Models.VnpayPaymentRequest
                {
                    Money = (double)totalOrderPrice,
                    BankCode = 0,
                    Description = order.PaymentId.ToString(),
                    Language = 0
                });
            }

            //Cycle Schedule Here
            if (isCycleSchedule)
            {
                var cycleSchedule = new Models.CycleSchedule
                {
                    CycleScheduleId = Guid.NewGuid(),
                    OrderId = order!.OrderId,
                    DayCycle = request.IsMonthly ? DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) : (int) request.DayCycle,
                    isMonthly = request.IsMonthly,
                    StartAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = "Active"
                };

                await cycleScheduleRepository.AddCycleSchedule(cycleSchedule);
            }

            var orderResponse = new PlaceOrderResponseDto
            {
                OrderId = order!.OrderId,
                CustomerName = order.Address.RecipientName,
                Address = order.Address.AddressDetail,
                Schedule = order.Schedule != null ? TimeZoneInfo.ConvertTimeFromUtc(order.Schedule.ScheduledDate, timeZone) : null,
                TotalPrice = totalOrderPrice,
                OrderDate = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, timeZone),
                OrderStatus = order.OrderStatus,    
                PaymentUrl = paymentUrl
            };

            return new ResultStatusWithData<PlaceOrderResponseDto>
            {
                Status = "Ok",
                Data = orderResponse
            };
        }

        //Get all orders
        public async Task<ResponseDtoWithPagination<List<OrderResponseDto>>> GetAllOrders(string accountEmail, int? page, int? size, string? keyword)
        {
            bool isPagination = false;
            int offset = 0;
            int pageSize = 0;
            if (page != null && size != null)
            {
                if (page <= 0)
                    throw new ArgumentException("Số trang (page) phải lớn hơn 0");
                if (size <= 0)
                    throw new ArgumentException("Kích thước trang (size) phải lớn hơn 0");

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
                    Schedule = order.Schedule != null ? TimeZoneInfo.ConvertTimeFromUtc(order.Schedule.ScheduledDate, timeZone): null,
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
                if (page <= 0)
                    throw new ArgumentException("Số trang (page) phải lớn hơn 0");
                if (size <= 0)
                    throw new ArgumentException("Kích thước trang (size) phải lớn hơn 0");

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
                Address = order.Address.AddressDetail,
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

        public async Task<ResponseDtoWithPagination<List<OrderResponseDto>>> GetAllOrdersAdmin(int? page, int? size, string? keyword)
        {
            bool isPagination = false;
            int offset = 0;
            int pageSize = 0;
            if (page != null && size != null)
            {
                if (page <= 0)
                    throw new ArgumentException("Số trang (page) phải lớn hơn 0");
                if (size <= 0)
                    throw new ArgumentException("Kích thước trang (size) phải lớn hơn 0");

                pageSize = (int)size;
                offset = (int)((page - 1) * size);
                isPagination = true;
            }

            var orders = await orderRepository.GetAllOrders();

            int totalItems = orders.Count;

            if (isPagination)
            {
                orders = await orderRepository.GetAllOrdersWithPagination(offset, pageSize);
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
                    Schedule = order.Schedule != null ? TimeZoneInfo.ConvertTimeFromUtc(order.Schedule.ScheduledDate, timeZone) : null,
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

        public async Task<ResponseDtoWithPagination<OrderDetailListResponseDto>> GetOrderDetailsAdmin(Guid orderId, int? page, int? size, string? keyword)
        {
            var order = await orderRepository.GetOrderByOrderId(orderId);
            if (order == null)
            {
                return null!;
            }

            bool isPagination = false;
            int offset = 0;
            int pageSize = 0;
            if (page != null && size != null)
            {
                if (page <= 0)
                    throw new ArgumentException("Số trang (page) phải lớn hơn 0");
                if (size <= 0)
                    throw new ArgumentException("Kích thước trang (size) phải lớn hơn 0");

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
                Address = order.Address.AddressDetail,
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

        public async Task<OrderResponseDto> UpdateOrderStatus(Guid orderId, UpdateOrderStatusRequestDto request)
        {
            var order = await orderRepository.GetOrderByOrderId(orderId);
            if (order == null)
            {
                return null!;
            }

            order.OrderStatus = request.Status;
            await orderRepository.UpdateOrder(order);

            //Handle Cycle Schedule After Complete Order Here
            if (order.OrderStatus.ToLower() == "completed")
            {
                var isCycleSchedule = await cycleScheduleRepository.CheckOrderIsCycleSchedule(order.OrderId);
                if (isCycleSchedule == true)
                {
                    var cycleSchedule = await cycleScheduleRepository.GetCycleScheduleByOrderId(order.OrderId);
                    cycleSchedule.UpdatedAt = DateTime.UtcNow;
                    cycleSchedule.DayCycle = cycleSchedule.isMonthly ? DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) : cycleSchedule.DayCycle;
                    
                    await cycleScheduleRepository.UpdateCycleSchedule(cycleSchedule);
                }
            }

            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                CustomerName = order.Address.RecipientName,
                Address = order.Address.AddressDetail,
                //Payment = (Guid)order.PaymentId,
                Schedule = order.Schedule != null ? TimeZoneInfo.ConvertTimeFromUtc(order.Schedule.ScheduledDate, timeZone) : null,
                TotalPrice = order.Payment.TotalAmount,
                OrderDate = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, timeZone),
                OrderStatus = order.OrderStatus
            };
        }

        private async Task<Models.Cart> GetCartByAccoutEmail(string accountEmail)
        {
            var account = await accountRepository.GetByEmailAsync(accountEmail);
            var customerId = account!.UserProfile.UserProfileId;
            var cart = await cartRepository.GetCartByCustomerId(customerId);

            return cart!;
        }

        public async Task<ResponseDtoWithPagination<List<OrderResponseDto>>> GetAllOrdersInSchedule(Guid scheduleId, int? page, int? size, string? keyword)
        {
            bool isPagination = false;
            int offset = 0;
            int pageSize = 0;
            if (page != null && size != null)
            {
                if (page <= 0)
                    throw new ArgumentException("Số trang (page) phải lớn hơn 0");
                if (size <= 0)
                    throw new ArgumentException("Kích thước trang (size) phải lớn hơn 0");

                pageSize = (int)size;
                offset = (int)((page - 1) * size);
                isPagination = true;
            }

            var orders = await orderRepository.GetAllOrdersInSchedule(scheduleId);

            int totalItems = orders.Count;

            if (isPagination)
            {
                orders = await orderRepository.GetAllOrdersInScheduleWithPagination(scheduleId, offset, pageSize);
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
                    Schedule = order.Schedule != null ? TimeZoneInfo.ConvertTimeFromUtc(order.Schedule.ScheduledDate, timeZone) : null,
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

        public async Task<ResultStatusWithData<OrderResponseDto>> UpdateOrderAddress(string accountEmail, Guid orderId, UpdateOrderAddressRequestDto request)
        {
            var order = await orderRepository.GetOrderByOrderId(orderId);
            if (order == null || order.Customer.Account.Email != accountEmail)
            {
                return new ResultStatusWithData<OrderResponseDto>
                {
                    Status = "Order 404",
                    Data = null
                };
            }

            if (order.OrderStatus != "Pending")
            {
                return new ResultStatusWithData<OrderResponseDto>
                {
                    Status = "Status Error",
                    Data = null
                };
            }

            var account = await accountRepository.GetByEmailAsync(accountEmail);
            var address = account!.UserProfile.Addresses.FirstOrDefault(a => a.AddressId == request.NewAddressId);
            if (address == null)
            {
                return new ResultStatusWithData<OrderResponseDto>
                {
                    Status = "Address 404",
                    Data = null
                };
            }

            order.AddressId = request.NewAddressId;

            await orderRepository.UpdateOrder(order);

            return new ResultStatusWithData<OrderResponseDto>
            {
                Status = "Ok",
                Data = new OrderResponseDto
                {
                    OrderId = order.OrderId,
                    CustomerName = order.Address.RecipientName,
                    Address = order.Address.AddressDetail,
                    //Payment = (Guid)order.PaymentId,
                    Schedule = order.Schedule != null ? TimeZoneInfo.ConvertTimeFromUtc(order.Schedule.ScheduledDate, timeZone) : null,
                    TotalPrice = order.Payment.TotalAmount,
                    OrderDate = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, timeZone),
                    OrderStatus = order.OrderStatus
                }
            };
        }

        public async Task<ResultStatusWithData<OrderResponseDto>> CancelOrder(string accountEmail, Guid orderId)
        {
            var order = await orderRepository.GetOrderByOrderId(orderId);
            if (order == null || order.Customer.Account.Email != accountEmail)
            {
                return new ResultStatusWithData<OrderResponseDto>
                {
                    Status = "Order 404",
                    Data = null
                };
            }

            if (order.OrderStatus != "Pending")
            {
                return new ResultStatusWithData<OrderResponseDto>
                {
                    Status = "Status Error",
                    Data = null
                };
            }

            order.OrderStatus = "Cancelled";
            var orderDetails = await orderDetailRepository.GetOrderDetailsByOrderId(orderId);
            foreach (var items in orderDetails)
            {
                var product = items.Product;
                product.Stock = product.Stock + items.Quantity;
                await productRepository.UpdateAsync(product);
            }

            await orderRepository.UpdateOrder(order);

            var isCycleSchedule = await cycleScheduleRepository.CheckOrderIsCycleSchedule(order.OrderId);
            if (isCycleSchedule == true)
            {
                var cycleSchedule = await cycleScheduleRepository.GetCycleScheduleByOrderId(order.OrderId);
                cycleSchedule.UpdatedAt = DateTime.UtcNow;
                cycleSchedule.DayCycle = cycleSchedule.isMonthly ? DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) : cycleSchedule.DayCycle;
                cycleSchedule.Status = "Inactive";

                await cycleScheduleRepository.UpdateCycleSchedule(cycleSchedule);
            }

            return new ResultStatusWithData<OrderResponseDto>
            {
                Status = "Ok",
                Data = new OrderResponseDto
                {
                    OrderId = order.OrderId,
                    CustomerName = order.Address.RecipientName,
                    Address = order.Address.AddressDetail,
                    //Payment = (Guid)order.PaymentId,
                    Schedule = order.Schedule != null ? TimeZoneInfo.ConvertTimeFromUtc(order.Schedule.ScheduledDate, timeZone) : null,
                    TotalPrice = order.Payment.TotalAmount,
                    OrderDate = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, timeZone),
                    OrderStatus = order.OrderStatus
                }
            };
        }
    }
}
