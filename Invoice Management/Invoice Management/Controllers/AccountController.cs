using Invoice_Management.Models.ViewModels;
using InvoiceManagement.Infrastructure__new_.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Invoice_Management.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public AccountController()
        {
        }

        // GET: Account/Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            var result = await signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                shouldLockout: false);

            switch (result)
            {
                case SignInStatus.Success:

                    var userManager = HttpContext.GetOwinContext().Get<ApplicationUserManager>();
                    var user = await userManager.FindByEmailAsync(model.Email);

                    if (await userManager.IsInRoleAsync(user.Id, "Admin"))
                        return RedirectToAction("Index", "User");

                    if (await userManager.IsInRoleAsync(user.Id, "Manager"))
                        return RedirectToAction("Dashboard", "Reports");

                    return RedirectToAction("Index", "Invoice");

                case SignInStatus.LockedOut:
                    return View("Lockout");

                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });

                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid email or password.");
                    return View(model);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            var signInManager = HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            signInManager.AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}