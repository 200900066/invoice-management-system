using System;

namespace Invoice_Management.Models.ViewModels.InvoiceModel
{
    public class ItemViewModel
    {
        public int ProductId { get; set; }
        public Guid InvoiceId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}