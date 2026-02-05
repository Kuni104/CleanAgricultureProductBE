namespace CleanAgricultureProductBE.DTOs.Cart
{
    public class AddToCartRequestDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

    }
}
