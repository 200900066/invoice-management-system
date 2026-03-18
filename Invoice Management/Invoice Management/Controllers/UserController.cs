using Invoice_Management.Models.ViewModels;
using InvoiceManagement.Infrastructure__new_.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Invoice_Management.Controllers
{

    // [Authorize(Roles = "Admin")]
    [AllowAnonymous]
    public class UserController : Controller
    {
        private ApplicationUserManager UserManager => HttpContext.GetOwinContext().Get<ApplicationUserManager>();
        private ApplicationRoleManager RoleManager => HttpContext.GetOwinContext().Get<ApplicationRoleManager>();

        public UserController()
        {
        }

        //public ActionResult Index()
        //{
        //    var userManager = HttpContext.GetOwinContext().Get<ApplicationUserManager>();
        //    var users = userManager.Users.ToList();
        //    return View(users);
        //}


        public ActionResult Index()
        {
            var users = UserManager.Users.ToList();

            var model = users.Select(u => new UserListViewModel
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FirstName + " " + u.LastName,
                PhoneNumber = u.PhoneNumber,
                Roles = UserManager.GetRoles(u.Id).FirstOrDefault()
            }).ToList();

            return View(model);
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

        public async Task<ActionResult> Edit(string id)
        {
            if (id == null) return HttpNotFound();

            var user = await UserManager.FindByIdAsync(id);
            if (user == null) return HttpNotFound();

            var roles = RoleManager.Roles.ToList();
            var userRoles = await UserManager.GetRolesAsync(user.Id);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                SelectedRole = userRoles.FirstOrDefault(),
                Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name,
                    Selected = userRoles.Contains(r.Name)
                }).ToList()
            };

            return View(model);
        }

        // UPDATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = RoleManager.Roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList();

                return View(model);
            }

            var user = await UserManager.FindByIdAsync(model.Id);
            if (user == null) return HttpNotFound();

            // Update fields
            user.Email = model.Email;
            user.UserName = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await UserManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Update roles
                var currentRoles = await UserManager.GetRolesAsync(user.Id);
                await UserManager.RemoveFromRolesAsync(user.Id, currentRoles.ToArray());
                await UserManager.AddToRoleAsync(user.Id, model.SelectedRole);

                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

            return View(model);
        }

        // DETAILS (OPTIONAL BUT GOOD PRACTICE)
        // =========================
        public async Task<ActionResult> Details(string id)
        {
            if (id == null) return HttpNotFound();

            var user = await UserManager.FindByIdAsync(id);
            if (user == null) return HttpNotFound();

            var model = new UserListViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FirstName + " " + user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = UserManager.GetRolesAsync(user.Id).Result.FirstOrDefault()
            };

            return View(model);
        }

        // =========================
        // DELETE (GET)
        // =========================
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null) return HttpNotFound();

            var user = await UserManager.FindByIdAsync(id);
            if (user == null) return HttpNotFound();

            var model = new UserListViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FirstName + " " + user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        // =========================
        // DELETE (POST)
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null) return HttpNotFound();

            await UserManager.DeleteAsync(user);

            return RedirectToAction("Index");
        }
    }
}