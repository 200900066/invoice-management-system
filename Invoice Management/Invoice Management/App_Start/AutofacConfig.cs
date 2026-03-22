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

            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterType<ApplicationDbContext>()
                   .InstancePerRequest();

            builder.RegisterType<UnitOfWork>()
                   .As<IUnitOfWork>()
                   .InstancePerRequest();

            builder.RegisterType<ProductService>()
                   .As<IProductService>()
                   .InstancePerRequest();

            builder.RegisterType<InvoiceService>()
                   .As<IInvoiceService>()
                   .InstancePerRequest();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}