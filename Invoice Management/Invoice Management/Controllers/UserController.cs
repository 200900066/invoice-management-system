using Invoice_Management.Models.ViewModels;
using InvoiceManagement.Infrastructure.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Invoice_Management.App_Start.IdentityConfig;

namespace Invoice_Management.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ??
                       HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ??
                       HttpContext.GetOwinContext().Get<ApplicationUserManager>();
            }
        }

        public UserController()
        {
        }

        public UserController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public ActionResult Index()
        
        {
            return View();
        }

        // GET: Create User
        public ActionResult Create()
        {
            var model = new CreateUserViewModel
            {
                Roles = RoleManager.Roles.ToList()
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
            if (!ModelState.IsValid)
            {
                model.Roles = _roleManager.Roles
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
                LastName = model.LastName     
            };

            var result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                //  ROLE ASSIGNMENT (THIS IS THE MAIN PART)
                await UserManager.AddToRoleAsync(user.Id, model.SelectedRole);

                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

            return View(model);
        }
    }
}