using InvoiceManagement.Infrastructure.Persistance;
using InvoiceManagement.Infrastructure__new_.Identity;
using Microsoft.Owin;
using Owin;
using static Invoice_Management.App_Start.IdentityConfig;

[assembly: OwinStartup(typeof(Invoice_Management.Startup))]
namespace Invoice_Management
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            // Seed roles on startup
            RoleInitializer.SeedRoles();
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            // Cookie authentication
            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
        }
    }
}