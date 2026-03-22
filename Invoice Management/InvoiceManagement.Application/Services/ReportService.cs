using InvoiceManagement.Application.Interface;
using InvoiceManagement.Infrastructure.Persistance;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceManagement.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReportData> GetReports()
        {
            // Total products
            var totalProducts = _context.Products.Count();

            // Total sold
            var totalProductsSold = await _context.InvoiceItems
                .SumAsync(ii => (int?)ii.Quantity) ?? 0;

            // Total stock
            var totalStock = await _context.Products
                .SumAsync(p => (int?)p.QuantityInStock) ?? 0;
            var productSales = await _context.InvoiceItems
                .Select(ii => new
                {
                    ii.ProductId,
                    ProductName = ii.Product.Name,
                    QuantityInStock = ii.Product.QuantityInStock,
                    Quantity = ii.Quantity
                })
                .GroupBy(x => new { x.ProductId, x.ProductName, x.QuantityInStock })
                .Select(g => new ProductSales
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    QuantityInStock = g.Key.QuantityInStock,
                    TotalItemsSold = g.Sum(x => x.Quantity)
                })
                .ToListAsync();

            return new ReportData
            {
                TotalProducts = totalProducts,
                TotalProductsSold = totalProductsSold,
                TotalStock = totalStock,
                TotalInStock = totalStock,
                TotalSold = totalProductsSold,
                ProductSales = productSales
            };
        }

        public async Task<string> GetLowStockNotification()
        {
            int threshold = 5;

            var lowStockItems = await _context.Products
                .Where(p => p.QuantityInStock <= threshold)
                .Select(p => new
                {
                    p.Name,
                    p.QuantityInStock
                })
                .ToListAsync();

            if (!lowStockItems.Any())
                return null;

            var message = "Low Stock: " + string.Join(", ",
                lowStockItems.Select(x => $"{x.Name} ({x.QuantityInStock})"));

            return message;
        }
    }
}
