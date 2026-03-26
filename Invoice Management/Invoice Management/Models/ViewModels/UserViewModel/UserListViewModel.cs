using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Invoice_Management.Models.ViewModels
{
    public class UserListViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public string PhoneNumber { get; set; }
        public string Roles { get; set; }
    }
}