using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Invoice_Management.Models.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string SelectedRole { get; set; }

        [Required]
        public string FirstName { get; set; }  

        [Required]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public List<SelectListItem> Roles { get; set; }
    }
}