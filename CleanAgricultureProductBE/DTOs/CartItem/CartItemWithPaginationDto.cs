using CleanAgricultureProductBE.DTOs.Response;

namespace CleanAgricultureProductBE.DTOs.CartItem
{
    public class CartItemWithPaginationDto
    {
        public GetCartItemsResponseWithTotalPrice? CartItemsResponseWithTotalPrice { get; set; }
        public Pagination? Pagination { get; set; }
    }
}
