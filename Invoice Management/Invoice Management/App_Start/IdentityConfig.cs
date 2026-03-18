using InvoiceManagement.Infrastructure.Identity;
using InvoiceManagement.Infrastructure.Persistance;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;


namespace Invoice_Management.App_Start
{
    public class IdentityConfig
    {
        public class ApplicationUserManager : UserManager<ApplicationUser>
        {
            public ApplicationUserManager(IUserStore<ApplicationUser> store)
                : base(store)
            {
            }

            public static ApplicationUserManager Create(
                IdentityFactoryOptions<ApplicationUserManager> options,
                IOwinContext context)
            {
                return new ApplicationUserManager(
                    new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>())
                );
            }
        }

        public class ApplicationRoleManager : RoleManager<IdentityRole>
        {
            public ApplicationRoleManager(IRoleStore<IdentityRole, string> store)
                : base(store)
            {
            }

            public static ApplicationRoleManager Create(
                IdentityFactoryOptions<ApplicationRoleManager> options,
                IOwinContext context)
            {
                return new ApplicationRoleManager(
                    new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>())
                );
            }
        }
    }
}