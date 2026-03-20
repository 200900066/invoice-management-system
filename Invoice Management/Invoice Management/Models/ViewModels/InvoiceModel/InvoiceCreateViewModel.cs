using InvoiceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Invoice_Management.Models.ViewModels.InvoiceModel
{
    public class InvoiceCreateViewModel
    {
        public List<InvoiceItemViewModel> Items { get; set; } = new List<InvoiceItemViewModel>();

        public IEnumerable<SelectListItem> ProductOptions { get; set; }

        public int SelectedProductId { get; set; }
        public int Quantity { get; set; }
        public Guid Id { get; set; }
    }
}