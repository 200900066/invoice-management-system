namespace Invoice_Management.Models.ViewModels
{
    public class ProductSalesViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public int StockRemaining { get; set; }
    }
}