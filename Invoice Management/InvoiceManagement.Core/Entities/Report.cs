using System.Collections.Generic;

namespace InvoiceManagement.Domain.Entities
{
    public class Report
    {
        public int TotalProducts { get; set; }
        public int TotalProductsSold { get; set; }
        public int TotalStock { get; set; }
        public int TotalInStock { get; set; }
        public int TotalSold { get; set; }
        public List<ProductSale> ItemsSoldPerProduct { get; set; }  
        public List<ProductSale> StockVsSoldPerProduct { get; set; }
    }
}