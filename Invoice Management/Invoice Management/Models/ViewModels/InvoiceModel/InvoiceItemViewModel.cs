using System;

namespace Invoice_Management.Models.ViewModels.InvoiceModel
{
    public class InvoiceItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
    }
}