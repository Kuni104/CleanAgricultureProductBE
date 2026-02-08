namespace CleanAgricultureProductBE.DTOs.CartItem
{
    public class CartItemsResponseWithTotalPriceDto
    {
        public List<CartItemResponseDto>? CartItemReponseList { get; set; }
        public decimal? TotalPriceOfAll { get; set; }
    }
}
