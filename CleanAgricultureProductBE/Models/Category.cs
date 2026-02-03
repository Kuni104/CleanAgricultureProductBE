namespace CleanAgricultureProductBE.Models
{
    public class Category
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        /*------------------------------------------------------------------------------------------------------------------------*/
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
