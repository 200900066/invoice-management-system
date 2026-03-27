using AutoMapper;
using Invoice_Management.Models.ViewModels;
using InvoiceManagement.Infrastructure__new_.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Serilog;
using System;
using System.Collections.Generic;
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

        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public UserController(ILogger logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            try
            {
                var users = UserManager.Users.ToList();

                var model = _mapper.Map<List<UserViewModel>>(users);

                // Assign roles separately
                foreach (var user in model)
                {
                    try
                    {
                        user.Roles = UserManager.GetRoles(user.Id).FirstOrDefault();
                    }
                    catch (Exception roleEx)
                    {
                        _logger.Error(roleEx, $"Error fetching role for user {user.Id}");
                        user.Roles = "N/A";
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading users");

                TempData["Error"] = "Failed to load users";

                return View(new List<UserViewModel>());
            }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateUserViewModel model)
        {
            var roleManager = HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            var userManager = HttpContext.GetOwinContext().Get<ApplicationUserManager>();

            try
            {
                if (!ModelState.IsValid)
                {
                    model.Roles = roleManager.Roles
                        .Select(r => new SelectListItem
                        {
                            Value = r.Name,
                            Text = r.Name
                        }).ToList();

                    TempData["Error"] = "Invalid form submission";
                    return View(model);
                }

                var user = _mapper.Map<ApplicationUser>(model);

                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    try
                    {
                        await userManager.AddToRoleAsync(user.Id, model.SelectedRole);
                    }
                    catch (Exception roleEx)
                    {
                        _logger.Error(roleEx, $"Error assigning role to user {user.Email}");
                    }

                    return RedirectToAction("Index");
                }

                // Handle identity errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                TempData["Error"] = "User creation failed";
                _logger.Warning($"User creation failed for {model.Email}: {string.Join(", ", result.Errors)}");

                model.Roles = roleManager.Roles
                    .Select(r => new SelectListItem
                    {
                        Value = r.Name,
                        Text = r.Name
                    }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error during user creation");
                TempData["Error"] = "Something went wrong while creating the user";

                return View(model);
            }
        }

        public async Task<ActionResult> Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Invalid user ID";
                    return RedirectToAction("Index");
                }

                var user = await UserManager.FindByIdAsync(id);

                if (user == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction("Index");
                }

                var roles = RoleManager.Roles.ToList();
                var userRoles = await UserManager.GetRolesAsync(user.Id);
                var model = _mapper.Map<EditUserViewModel>(user);
                model.SelectedRole = userRoles.FirstOrDefault();

                model.Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name,
                    Selected = userRoles.Contains(r.Name)
                }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error loading user for edit: {id}");

                TempData["Error"] = "Something went wrong while loading the user";

                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.Roles = RoleManager.Roles.Select(r => new SelectListItem
                    {
                        Value = r.Name,
                        Text = r.Name
                    }).ToList();

                    TempData["Error"] = "Invalid form submission";
                    return View(model);
                }

                var user = await UserManager.FindByIdAsync(model.Id);

                if (user == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction("Index");
                }

                _mapper.Map(model, user);

                var result = await UserManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    try
                    {
                        var currentRoles = await UserManager.GetRolesAsync(user.Id);

                        if (currentRoles.Any())
                        {
                            await UserManager.RemoveFromRolesAsync(user.Id, currentRoles.ToArray());
                        }

                        if (!string.IsNullOrEmpty(model.SelectedRole))
                        {
                            await UserManager.AddToRoleAsync(user.Id, model.SelectedRole);
                        }
                    }
                    catch (Exception roleEx)
                    {
                        _logger.Error(roleEx, $"Error updating roles for user {user.Email}");
                        TempData["Error"] = "User updated but role assignment failed";
                        return RedirectToAction("Index");
                    }

                    TempData["Success"] = "User updated successfully";
                    return RedirectToAction("Index");
                }

                // Identity errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                _logger.Warning($"User update failed for {model.Email}: {string.Join(", ", result.Errors)}");

                TempData["Error"] = "User update failed";

                model.Roles = RoleManager.Roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Unexpected error updating user {model.Email}");

                TempData["Error"] = "Something went wrong while updating the user";

                model.Roles = RoleManager.Roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList();

                return View(model);
            }
        }

        public async Task<ActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Invalid user ID";
                    return RedirectToAction("Index");
                }

                var user = await UserManager.FindByIdAsync(id);

                if (user == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction("Index");
                }

                var model = _mapper.Map<UserViewModel>(user);
                var roles = await UserManager.GetRolesAsync(user.Id);
                model.Roles = roles.FirstOrDefault();

                TempData["Success"] = "User details loaded";

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error loading user details for ID: {id}");

                TempData["Error"] = "Something went wrong while loading user details";

                return RedirectToAction("Index");
            }
        }

        //public async Task<ActionResult> Delete(string id)
        //{
        //    if (id == null) return HttpNotFound();

        //    var user = await UserManager.FindByIdAsync(id);
        //    if (user == null) return HttpNotFound();

        //    var model = new UserViewModel
        //    {
        //        Id = user.Id,
        //        Email = user.Email,
        //        FullName = user.FirstName + " " + user.LastName,
        //        PhoneNumber = user.PhoneNumber
        //    };

        //    return View(model);
        //}

        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Invalid user ID";
                    return RedirectToAction("Index");
                }

                var user = await UserManager.FindByIdAsync(id);

                if (user == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction("Index");
                }

                var model = _mapper.Map<UserViewModel>(user);

                TempData["Warning"] = "You are about to delete this user";

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error loading delete confirmation for user {id}");

                TempData["Error"] = "Something went wrong";

                return RedirectToAction("Index");
            }
        }

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(string id)
        //{
        //    var user = await UserManager.FindByIdAsync(id);
        //    if (user == null) return HttpNotFound();

        //    await UserManager.DeleteAsync(user);

        //    return RedirectToAction("Index");
        //}

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "Invalid user ID";
                    return RedirectToAction("Index");
                }

                var user = await UserManager.FindByIdAsync(id);

                if (user == null)
                {
                    TempData["Error"] = "User not found";
                    return RedirectToAction("Index");
                }

                if (user.Id == User.Identity.GetUserId())
                {
                    TempData["Error"] = "You cannot delete your own account";
                    return RedirectToAction("Index");
                }

                var roles = await UserManager.GetRolesAsync(user.Id);
                if (roles.Contains("Admin"))
                {
                    TempData["Error"] = "Admin users cannot be deleted";
                    return RedirectToAction("Index");
                }
                
                await UserManager.DeleteAsync(user);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error deleting user with ID {id}");

                TempData["Error"] = "Something went wrong while deleting the user";

                return RedirectToAction("Index");
            }
        }
    }
}