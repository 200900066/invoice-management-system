using InvoiceManagement.Infrastructure.Persistance;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace InvoiceManagement.Infrastructure__new_.Identity
{
    
    public static class RoleInitializer
    {
        public static void SeedRoles()
        {
            using (var context = new ApplicationDbContext())
            {
                var roleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));

                // Check and create roles
                if (!roleManager.RoleExists(Roles.Admin))
                    roleManager.Create(new IdentityRole(Roles.Admin));

                if (!roleManager.RoleExists(Roles.Manager))
                    roleManager.Create(new IdentityRole(Roles.Manager));

                if (!roleManager.RoleExists(Roles.User))
                    roleManager.Create(new IdentityRole(Roles.User));
            }
        }
    }
}
