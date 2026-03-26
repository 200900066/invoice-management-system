namespace Invoice_Management.Models.ViewModels.ReportViewModel
{
    public class ProductSaleViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalItemsSold { get; set; }
        public int QuantityInStock { get; set; }
    }
}