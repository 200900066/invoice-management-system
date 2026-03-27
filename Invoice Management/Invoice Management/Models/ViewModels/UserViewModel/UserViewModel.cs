namespace Invoice_Management.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public string PhoneNumber { get; set; }
        public string Roles { get; set; }
    }
}