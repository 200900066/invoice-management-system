using System;
using System.Collections.Generic;

namespace Invoice_Management.Models.ViewModels.InvoiceModel
{
    public class InvoiceViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserName { get; set; }
        public decimal Total { get; set; } 
        public List<InvoiceItemViewModel> Items { get; set; } = new List<InvoiceItemViewModel>();
    }
}