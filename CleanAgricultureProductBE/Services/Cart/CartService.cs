using CleanAgricultureProductBE.DTOs.Cart;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories.Cart;
using CleanAgricultureProductBE.Repositories.CartItem;
using CleanAgricultureProductBE.DTOs.CartItem;
using CleanAgricultureProductBE.DTOs.Response;

namespace CleanAgricultureProductBE.Services.Cart
{
    public class CartService(IAccountRepository accountRepository, IProductRepository productRepository, ICartRepository cartRepository, ICartItemRepository cartItemRepository) : ICartService
    {

        public async Task<AddToCartResponseDto> AddToCart(string accountEmail, AddToCartRequestDto request)
        {
            var account = await accountRepository.GetByEmailAsync(accountEmail);
            var customerId = account!.UserProfile.UserProfileId;

            var cart = await cartRepository.GetCartByCustomerId(customerId);
            var product = await productRepository.GetByIdAsync(request.ProductId);

            var cartItem = await cartItemRepository.GetCartItemByCartIdAndProductId(cart!.CartId, product!.ProductId);

            if (cartItem is null)
            {
                cartItem = new CartItem
                {
                    CartItemId = Guid.NewGuid(),
                    CartId = cart!.CartId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    CreatedAt = DateTime.UtcNow
                };

                await cartItemRepository.AddCartItem(cartItem);
            }
            else
            {
                cartItem.Quantity += request.Quantity;
                await cartItemRepository.UpdateCartItem(cartItem);
            }

            return new AddToCartResponseDto
            {
                CartItemId = cartItem.CartItemId,
                CartId = cart.CartId,
                ProductId = request.ProductId,
                Quantity = cartItem.Quantity,
                TotalPrice = product!.Price * cartItem.Quantity
            };
        }
        public async Task<CartItemWithPaginationDto> GetCartItem(string accountEmail, int? page, int? size, string? keyword)
        {
            bool isPagination = false;
            int offset = 0;
            int pageSize = 0;
            if (page != null && size != null)
            {
                pageSize = (int) size;
                offset = (int) ((page - 1) * size);
                isPagination = true;
            }

            var account = await accountRepository.GetByEmailAsync(accountEmail);
            var customerId = account!.UserProfile.UserProfileId;
            var cart = await cartRepository.GetCartByCustomerId(customerId);

            var cartItemList = await cartItemRepository.GetCartItemsByCartId(cart!.CartId);

            int totalItems = cartItemList.Count;

            if (isPagination) {
                cartItemList = await cartItemRepository.GetCartItemsByCartIdWithPagination(cart!.CartId, offset, pageSize);
            }


            List<GetCartItemReponseDto> cartItems = new List<GetCartItemReponseDto>();

            foreach (var item in cartItemList)
            {
                var product = await productRepository.GetByIdAsync(item.ProductId);
                cartItems.Add(new GetCartItemReponseDto
                {
                    ProductId = product!.ProductId,
                    ProductName = product!.Name,
                    Price = product.Price,
                    Quantity = item.Quantity,
                    TotalPrice = product.Price * item.Quantity
                });
            }

            decimal total = 0;

            foreach (var item in cartItems)
            {
                total = total + item.TotalPrice;
            }


            var cartItemWithTotalPrice = new GetCartItemsResponseWithTotalPrice
            {
                CartItemReponseList = cartItems,
                TotalPriceOfAll = total
            };

            var result = new CartItemWithPaginationDto
            {
                CartItemsResponseWithTotalPrice = cartItemWithTotalPrice
            };


            if (isPagination)
            {
                int totalPage = totalItems / pageSize + (totalItems % pageSize == 0 ? 1 : 0);
                if (totalItems < pageSize)
                {
                    totalPage = 1;
                }

                result.Pagination = new Pagination
                {
                    PageNumber = (int) page!,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPage
                };
            }

            return result;
        }

        public async Task<GetCartItemReponseDto> UpdateCartItem(string accountEmail, UpdateCartItemRequestDto request)
        {
            var account = await accountRepository.GetByEmailAsync(accountEmail);
            var customerId = account!.UserProfile.UserProfileId;
            var cart = await cartRepository.GetCartByCustomerId(customerId);

            var cartItem = await cartItemRepository.GetCartItemByCartIdAndProductId(cart!.CartId, request.ProductId);
            var product = await productRepository.GetByIdAsync(request.ProductId);

            cartItem!.Quantity = request.Quanity;

            await cartItemRepository.UpdateCartItem(cartItem);

            var result = new GetCartItemReponseDto
            {
                ProductId = cartItem.ProductId,
                ProductName = cartItem.Product.Name,
                Price = product!.Price,
                Quantity = cartItem.Quantity,
                TotalPrice = product.Price * cartItem.Quantity
            };

            return result;
        }
    }
}
