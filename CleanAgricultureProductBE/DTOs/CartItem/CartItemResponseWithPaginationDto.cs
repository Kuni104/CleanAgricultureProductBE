using CleanAgricultureProductBE.DTOs.Response;

namespace CleanAgricultureProductBE.DTOs.CartItem
{
    public class CartItemResponseWithPaginationDto
    {
        public CartItemsResponseWithTotalPriceDto? CartItemsResponseWithTotalPrice { get; set; }
        public Pagination? Pagination { get; set; }
    }
}
