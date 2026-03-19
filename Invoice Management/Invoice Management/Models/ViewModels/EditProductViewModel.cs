using System;
using System.ComponentModel.DataAnnotations;

namespace Invoice_Management.Models.ViewModels
{
    public class EditProductViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal CostPerItem { get; set; }

        [Required]
        public int QuantityInStock { get; set; }
    }
}