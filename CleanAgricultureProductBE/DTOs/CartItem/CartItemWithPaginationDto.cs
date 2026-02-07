using CleanAgricultureProductBE.DTOs.Response;

namespace CleanAgricultureProductBE.DTOs.CartItem
{
    public class CartItemWithPaginationDto
    {
        public List<GetCartItemReponseDto>? CartItemReponseList {  get; set; }
        public Pagination? Pagination { get; set; }
    }
}
