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

            // Per product
            var productSales = await _context.InvoiceItems
                .GroupBy(ii => new { ii.ProductId, ii.Product.Name })
                .Select(g => new ProductSales
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
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
    }
}
