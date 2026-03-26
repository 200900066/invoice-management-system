using Invoice_Management.Models.ViewModels.ReportViewModel;
using System.Collections.Generic;

namespace Invoice_Management.Models.ViewModels
{
    public class ReportsViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalProductsSold { get; set; }
        public int TotalStock { get; set; }
        public List<ProductSaleViewModel> ItemsSoldPerProduct { get; set; } = new List<ProductSaleViewModel>();
        public List<ProductSaleViewModel> StockVsSoldPerProduct { get; set; } = new List<ProductSaleViewModel>();
    }
}