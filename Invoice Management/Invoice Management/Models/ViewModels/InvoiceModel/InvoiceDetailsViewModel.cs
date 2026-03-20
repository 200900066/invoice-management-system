using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Invoice_Management.Models.ViewModels.InvoiceModel
{
    public class InvoiceDetailsViewModel
    {
        public Guid Id { get; set; }
        public decimal Total { get; set; }

        public List<InvoiceItemViewModel> Items { get; set; }

    }
}