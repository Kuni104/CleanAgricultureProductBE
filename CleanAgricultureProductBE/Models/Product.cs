namespace CleanAgricultureProductBE.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Unit { get; set; }
        public int Stock { get; set; }
        public string Status { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }

        public Category Category { get; set; } = null!;
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public ICollection<ProductComplaint> ProductComplaints { get; set; } = new List<ProductComplaint>();
    }
}
