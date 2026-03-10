using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.DTOs.Product;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Services.Product
{
    public class ProductStatisticsService : IProductStatisticsService
    {
        private readonly AppDbContext _context;

        public ProductStatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductStatisticsDto> GetProductStatisticsAsync(int? year, int? month, int topN)
        {
            var targetYear = year ?? DateTime.Now.Year;

            // Product counts (server-side)
            var totalProducts = await _context.Products.IgnoreQueryFilters()
                .CountAsync(p => !p.IsDeleted);
            var activeProducts = await _context.Products.IgnoreQueryFilters()
                .CountAsync(p => !p.IsDeleted && p.Status == "Active");
            var inactiveProducts = await _context.Products.IgnoreQueryFilters()
                .CountAsync(p => !p.IsDeleted && p.Status == "Inactive");
            var outOfStockProducts = await _context.Products.IgnoreQueryFilters()
                .CountAsync(p => !p.IsDeleted && p.Stock <= 0);

            var totalCategories = await _context.Categories.CountAsync();

            // Base query: completed order details
            var baseQuery = _context.OrderDetails
                .Where(od => od.Order.OrderStatus == "Completed");

            // Apply time filter
            if (month.HasValue)
            {
                baseQuery = baseQuery.Where(od =>
                    od.Order.OrderDate.Year == targetYear &&
                    od.Order.OrderDate.Month == month.Value);
            }
            else
            {
                baseQuery = baseQuery.Where(od =>
                    od.Order.OrderDate.Year == targetYear);
            }

            // Totals (server-side aggregation)
            var totalProductsSold = await baseQuery.SumAsync(od => od.Quantity);
            var totalRevenue = await baseQuery.SumAsync(od => od.TotalPrice);

            // Category statistics (server-side)
            var categoryStats = await baseQuery
                .GroupBy(od => new { od.Product.CategoryId, od.Product.Category.Name })
                .Select(g => new CategoryStatisticsDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    ProductCount = 0, // filled below
                    TotalSold = g.Sum(od => od.Quantity),
                    Revenue = g.Sum(od => od.TotalPrice)
                })
                .OrderByDescending(c => c.Revenue)
                .ToListAsync();

            // Product count per category (server-side)
            var productCountByCategory = await _context.Products.IgnoreQueryFilters()
                .Where(p => !p.IsDeleted)
                .GroupBy(p => p.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CategoryId, x => x.Count);

            foreach (var cs in categoryStats)
            {
                cs.ProductCount = productCountByCategory.GetValueOrDefault(cs.CategoryId, 0);
            }

            // Categories with no sales
            var categoriesWithSalesIds = categoryStats.Select(c => c.CategoryId).ToHashSet();
            var categoriesWithoutSales = await _context.Categories
                .Where(c => !categoriesWithSalesIds.Contains(c.CategoryId))
                .Select(c => new { c.CategoryId, c.Name })
                .ToListAsync();

            foreach (var c in categoriesWithoutSales)
            {
                categoryStats.Add(new CategoryStatisticsDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.Name,
                    ProductCount = productCountByCategory.GetValueOrDefault(c.CategoryId, 0),
                    TotalSold = 0,
                    Revenue = 0
                });
            }

            // Top selling products (server-side)
            var topSellingProducts = await baseQuery
                .GroupBy(od => new
                {
                    od.ProductId,
                    od.Product.Name,
                    CategoryName = od.Product.Category.Name,
                    od.Product.Price,
                    od.Product.Unit
                })
                .Select(g => new TopSellingProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    CategoryName = g.Key.CategoryName,
                    Price = g.Key.Price,
                    Unit = g.Key.Unit,
                    TotalSold = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => od.TotalPrice)
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(topN)
                .ToListAsync();

            // Revenue by month
            var revenueQuery = _context.OrderDetails
                .Where(od => od.Order.OrderStatus == "Completed" &&
                        od.Order.OrderDate.Year == targetYear);

            if (month.HasValue)
            {
                revenueQuery = revenueQuery.Where(od => od.Order.OrderDate.Month == month.Value);
            }

            var revenueByMonth = await revenueQuery
                .GroupBy(od => od.Order.OrderDate.Month)
                .Select(g => new RevenueByMonthDto
                {
                    Year = targetYear,
                    Month = g.Key,
                    OrderCount = g.Select(od => od.OrderId).Distinct().Count(),
                    ProductsSold = g.Sum(od => od.Quantity),
                    Revenue = g.Sum(od => od.TotalPrice)
                })
                .ToListAsync();

            // Fill missing months
            var existingMonths = revenueByMonth.Select(r => r.Month).ToHashSet();
            var startMonth = month ?? 1;
            var endMonth = month ?? 12;
            for (int m = startMonth; m <= endMonth; m++)
            {
                if (!existingMonths.Contains(m))
                {
                    revenueByMonth.Add(new RevenueByMonthDto
                    {
                        Year = targetYear,
                        Month = m,
                        OrderCount = 0,
                        ProductsSold = 0,
                        Revenue = 0
                    });
                }
            }
            revenueByMonth = revenueByMonth.OrderBy(r => r.Month).ToList();

            return new ProductStatisticsDto
            {
                TotalProducts = totalProducts,
                ActiveProducts = activeProducts,
                InactiveProducts = inactiveProducts,
                OutOfStockProducts = outOfStockProducts,
                TotalCategories = totalCategories,
                TotalProductsSold = totalProductsSold,
                TotalRevenue = totalRevenue,
                CategoryStatistics = categoryStats,
                TopSellingProducts = topSellingProducts,
                RevenueByMonth = revenueByMonth
            };
        }
    }
}
