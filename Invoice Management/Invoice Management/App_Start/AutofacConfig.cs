using Autofac;
using Autofac.Integration.Mvc;
using InvoiceManagement.Application.Interface;
using InvoiceManagement.Application.Services;
using InvoiceManagement.Infrastructure.Interface;
using InvoiceManagement.Infrastructure.Persistance;
using InvoiceManagement.Infrastructure.UnitOfWork;
using System.Web.Mvc;

namespace Invoice_Management.App_Start
{
    public class AutofacConfig
    {
        public static void Register()
        {
            var builder = new ContainerBuilder();

            // Register controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // DbContext per request
            builder.RegisterType<ApplicationDbContext>()
                   .InstancePerRequest();

            // Unit of Work
            builder.RegisterType<UnitOfWork>()
                   .As<IUnitOfWork>()
                   .InstancePerRequest();

            // Services
            builder.RegisterType<ProductService>()
                   .As<IProductService>()
                   .InstancePerRequest();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}