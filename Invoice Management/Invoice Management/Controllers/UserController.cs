using Invoice_Management.Models.ViewModels;
using InvoiceManagement.Infrastructure.Identity;
using InvoiceManagement.Infrastructure__new_.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Invoice_Management.Controllers
{

    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        public UserController()
        {
        }

        public ActionResult Index()
        {
            var userManager = HttpContext.GetOwinContext().Get<ApplicationUserManager>();
            var users = userManager.Users.ToList();
            return View(users);
        }


        public ActionResult Create()
        {
            var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            var model = new CreateUserViewModel
            {
                Roles = roleManager.Roles.ToList()
                    .Select(r => new SelectListItem
                    {
                        Value = r.Name,
                        Text = r.Name
                    }).ToList()
            };

            return View(model);
        }

        // POST: Create User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateUserViewModel model)
        {
            var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            if (!ModelState.IsValid)
            {
                model.Roles = roleManager.Roles
                    .Select(r => new SelectListItem
                    {
                        Value = r.Name,
                        Text = r.Name
                    }).ToList();

                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber
            };

            var userManager = HttpContext .GetOwinContext().Get<ApplicationUserManager>();
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                //  ROLE ASSIGNMENT (THIS IS THE MAIN PART)
                await userManager.AddToRoleAsync(user.Id, model.SelectedRole);

                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

            return View(model);
        }
    }
}