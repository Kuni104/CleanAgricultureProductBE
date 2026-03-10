namespace CleanAgricultureProductBE.DTOs.Product
{
    public class ProductStatisticsDto
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int InactiveProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalProductsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<CategoryStatisticsDto> CategoryStatistics { get; set; } = new();
        public List<TopSellingProductDto> TopSellingProducts { get; set; } = new();
        public List<RevenueByMonthDto> RevenueByMonth { get; set; } = new();
    }

    public class CategoryStatisticsDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public int TotalSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class TopSellingProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Unit { get; set; } = string.Empty;
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RevenueByMonthDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int OrderCount { get; set; }
        public int ProductsSold { get; set; }
        public decimal Revenue { get; set; }
    }
}
