using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using Invoice_Management.Mappings;
using InvoiceManagement.Application.Interface;
using InvoiceManagement.Application.Services;
using InvoiceManagement.Infrastructure.Interface;
using InvoiceManagement.Infrastructure.Persistance;
using InvoiceManagement.Infrastructure.UnitOfWork;
using Serilog;
using System;
using System.Collections.Generic;
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

            builder.RegisterType<ReportService>()
                  .As<IReportService>()
                  .InstancePerRequest();

            builder.RegisterType<MappingProfile>().As<Profile>();

            // Mapper configuration
            builder.Register(ctx =>
            {
                var profiles = ctx.Resolve<IEnumerable<Profile>>();

                var config = new MapperConfiguration(cfg =>
                {
                    foreach (var profile in profiles)
                    {
                        cfg.AddProfile(profile);
                    }
                });

                return config;
            })
            .AsSelf()
            .SingleInstance();

            // IMapper
            builder.Register(ctx =>
            {
                var config = ctx.Resolve<MapperConfiguration>();
                return config.CreateMapper();
            })
            .As<IMapper>()
            .InstancePerRequest();

            builder.RegisterInstance(Log.Logger).As<Serilog.ILogger>().SingleInstance();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}