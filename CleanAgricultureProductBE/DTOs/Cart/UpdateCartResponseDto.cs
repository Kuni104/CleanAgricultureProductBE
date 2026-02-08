using CleanAgricultureProductBE.DTOs.CartItem;

namespace CleanAgricultureProductBE.DTOs.Cart
{
    public class UpdateCartResponseDto
    {
        public CartItemResponseDto? CartItemReponse { get; set; }
        public decimal? TotalPriceOfAll { get; set; }
    }
}
