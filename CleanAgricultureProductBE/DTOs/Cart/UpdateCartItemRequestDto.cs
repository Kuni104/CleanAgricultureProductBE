namespace CleanAgricultureProductBE.DTOs.Cart
{
    public class UpdateCartItemRequestDto
    {
        public Guid ProductId { get; set; }
        public int Quanity { get; set; }
    }
}
