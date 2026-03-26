namespace InvoiceManagement.Domain.Entities
{
    public class ProductSale
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalItemsSold { get; set; }
        public int QuantityInStock { get; set; }
    }
}
