using CleanAgricultureProductBE.DTOs.Cart;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Models;
using CleanAgricultureProductBE.Repositories.Cart;
using CleanAgricultureProductBE.Repositories.CartItem;
using CleanAgricultureProductBE.DTOs.CartItem;
using CleanAgricultureProductBE.DTOs.Response;
using CleanAgricultureProductBE.DTOs;
using CleanAgricultureProductBE.DTOs.ApiResponse;

namespace CleanAgricultureProductBE.Services.Cart
{
    public class CartService(IAccountRepository accountRepository,IProductService productService, ICartRepository cartRepository) : ICartService
    {

        public async Task<AddToCartResponseDto> AddToCart(string accountEmail, Guid productId, CartRequestDto request)
        {
            var cart = await GetCartByAccoutEmail(accountEmail);

            //var product = await productRepository.GetByIdAsync(productId);
            var product = await productService.GetProductByIdAsync(productId);

            var cartItem = await cartRepository.GetCartItemByCartIdAndProductId(cart!.CartId, product!.ProductId);

            if (cartItem is null)
            {
                cartItem = new CartItem
                {
                    CartItemId = Guid.NewGuid(),
                    CartId = cart!.CartId,
                    ProductId = productId,
                    UnitPrice = product.Price,
                    Quantity = request.Quantity,
                    TotalPrice = product.Price * request.Quantity,
                    CreatedAt = DateTime.UtcNow
                };

                await cartRepository.AddCartItem(cartItem);
            }
            else
            {
                cartItem.Quantity += request.Quantity;
                cartItem.TotalPrice = cartItem.UnitPrice * request.Quantity;
                await cartRepository.UpdateCartItem(cartItem);
            }

            return new AddToCartResponseDto
            {
                CartItemId = cartItem.CartItemId,
                CartId = cart.CartId,
                ProductId = productId,
                UnitPrice = cartItem.UnitPrice,
                Quantity = cartItem.Quantity,
                TotalPrice = cartItem.TotalPrice
            };
        }

        public async Task<CartItemResponseWithPaginationDto> GetCartItem(string accountEmail, int? page, int? size, string? keyword)
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

            var cart = await GetCartByAccoutEmail(accountEmail);

            var cartItemList = await cartRepository.GetCartItemsByCartIdIncludeProduct(cart!.CartId);

            int totalItems = cartItemList.Count;

            if (isPagination) {
                cartItemList = await cartRepository.GetCartItemsByCartIdWithPagination(cart!.CartId, offset, pageSize);
            }


            List<CartItemResponseDto> cartItems = new List<CartItemResponseDto>();

            foreach (var item in cartItemList)
            {
                cartItems.Add(new CartItemResponseDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Price = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                });
            }

            decimal total = 0;

            foreach (var item in cartItems)
            {
                total = total + item.TotalPrice;
            }


            var cartItemWithTotalPrice = new CartItemsResponseWithTotalPriceDto
            {
                CartItemReponseList = cartItems,
                TotalPriceOfAll = total
            };

            var result = new CartItemResponseWithPaginationDto
            {
                CartItemsResponseWithTotalPrice = cartItemWithTotalPrice
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
                    PageNumber = (int) page!,
                    PageSize = pageSize,
                    TotalItems = totalItems,
                    TotalPages = totalPage
                };
            }

            return result;
        }

        public async Task<UpdateCartResponseDto> UpdateCartItemQuantity(string accountEmail, Guid productId, CartRequestDto request)
        {
            var cart = await GetCartByAccoutEmail(accountEmail);

            var cartItem = await cartRepository.GetCartItemByCartIdAndProductIdIncludeProduct(cart!.CartId, productId);
            //var product = await productService.GetProductByIdAsync(productId);

            cartItem!.Quantity = request.Quantity;
            cartItem!.TotalPrice = cartItem.UnitPrice * request.Quantity;

            await cartRepository.UpdateCartItem(cartItem);

            var cartItemRespsonse = new CartItemResponseDto
            {
                ProductId = cartItem.ProductId,
                ProductName = cartItem.Product.Name,
                Price = cartItem.UnitPrice,
                Quantity = cartItem.Quantity,
                TotalPrice = cartItem.TotalPrice
            };

            var totalPrice = await TotalPriceOfCartByCartId(cart!.CartId);

            var result = new UpdateCartResponseDto
            {
                CartItemReponse = cartItemRespsonse,
                TotalPriceOfAll = totalPrice
            };

            return result;
        }


        public async Task<ResultStatusWithData<decimal>> DeleteCartItem(string accountEmail, Guid productId)
        {
            var cart = await GetCartByAccoutEmail(accountEmail);
            var cartItem = await cartRepository.GetCartItemByCartIdAndProductId(cart!.CartId, productId);

            if (cartItem == null)
            {
                return new ResultStatusWithData<decimal>
                {
                    Status = "ID 404"
                };
            }

            await cartRepository.DeleteCartItem(cartItem!);

            var totalPrice = await TotalPriceOfCartByCartId(cart!.CartId);

            return new ResultStatusWithData<decimal>
            {
                Status = "OK",
                Data = totalPrice
            };
        }

        public async Task<string> DeleteAllCartItems(string accountEmail)
        {
            var cart = await GetCartByAccoutEmail(accountEmail);

            if (cart == null)
            {
                return "404 Cart";
            }

            await cartRepository.DeleteAllCartItems(cart.CartId);

            return "OK";
        }

        private async Task<Models.Cart> GetCartByAccoutEmail(string accountEmail)
        {
            var account = await accountRepository.GetByEmailAsync(accountEmail);
            var customerId = account!.UserProfile.UserProfileId;
            var cart = await cartRepository.GetCartByCustomerId(customerId);

            return cart!;
        }

        public async Task<decimal> TotalPriceOfCartByCartId(Guid cartId)
        {
            return await cartRepository.TotalPriceOfCartByCartId(cartId);
        }
    }
}
