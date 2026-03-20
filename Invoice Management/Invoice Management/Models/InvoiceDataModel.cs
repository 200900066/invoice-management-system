using Invoice_Management.Models.ViewModels;
using InvoiceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invoice_Management.Models
{
    public class InvoiceDataModel
    {
        public List<Product> Products { get; set; }
        public Invoice Invoice { get; set; }
    }
}