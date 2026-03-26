using Invoice_Management.Models.ViewModels;
using Invoice_Management.Models.ViewModels.InvoiceModel;
using System.Collections.Generic;

namespace Invoice_Management.Models
{
    public class InvoiceDataViewModel
    {
        public List<ProductViewModel> Products { get; set; }
        public InvoiceViewModel Invoice { get; set; }
    }
}