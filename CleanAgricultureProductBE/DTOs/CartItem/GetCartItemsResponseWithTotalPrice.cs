namespace CleanAgricultureProductBE.DTOs.CartItem
{
    public class GetCartItemsResponseWithTotalPrice
    {
        public List<GetCartItemResponseDto>? CartItemReponseList { get; set; }
        public decimal? TotalPriceOfAll { get; set; }
    }
}
