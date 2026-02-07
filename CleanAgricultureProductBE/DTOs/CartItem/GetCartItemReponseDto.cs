namespace CleanAgricultureProductBE.DTOs.CartItem
{
    public class GetCartItemReponseDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
