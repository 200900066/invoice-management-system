using System.Collections.Generic;

namespace InvoiceManagement.Application.Services
{
    public class ReportData
    {
        public int TotalProducts { get; set; }
        public int TotalProductsSold { get; set; }
        public int TotalStock { get; set; }
        public int TotalInStock { get; set; }
        public int TotalSold { get; set; }
        public List<ProductSales> ProductSales { get; set; }
    }
}