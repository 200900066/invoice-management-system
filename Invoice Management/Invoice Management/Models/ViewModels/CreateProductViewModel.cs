using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Invoice_Management.Models.ViewModels
{
    public class CreateProductViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0")]
        public decimal CostPerItem { get; set; }

        [Required]
        [Range(0, 100000, ErrorMessage = "Quantity cannot be negative")]
        public int QuantityInStock { get; set; }
    }
}