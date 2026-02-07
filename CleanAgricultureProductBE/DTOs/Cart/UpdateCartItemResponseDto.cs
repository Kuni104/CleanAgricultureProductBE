using CleanAgricultureProductBE.DTOs.CartItem;

namespace CleanAgricultureProductBE.DTOs.Cart
{
    public class UpdateCartItemResponseDto
    {
        public GetCartItemResponseDto? CartItemReponse { get; set; }
        public decimal? TotalPriceOfAll { get; set; }
    }
}
