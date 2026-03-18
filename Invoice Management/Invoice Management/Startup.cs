using InvoiceManagement.Infrastructure__new_.Identity;
using Microsoft.Owin;
using Owin;

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