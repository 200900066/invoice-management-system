using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invoice_Management.Models.ViewModels
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal CostPerItem { get; set; }
        public int QuantityInStock { get; set; }
    }
}