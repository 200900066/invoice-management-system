using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invoice_Management.Models.ViewModels
{
    public class ReportsViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalProductsSold { get; set; }
        public int TotalStock { get; set; }
        public int TotalInStock { get; set; }
        public int TotalSold { get; set; }
        public List<ProductSalesViewModel> ProductSales { get; set; }
    }
}