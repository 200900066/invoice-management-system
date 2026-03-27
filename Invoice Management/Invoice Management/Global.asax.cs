using Invoice_Management.App_Start;
using Serilog;
using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Invoice_Management
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

            // Ensure folder exists
            Directory.CreateDirectory(logDir);

            var logPath = Path.Combine(logDir, "log-.txt");

            //  CONFIG (THIS WAS MISSING FILE SINK)
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() 
                .WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7
                )
                .CreateLogger();

            Log.Information("Application started");
            AreaRegistration.RegisterAllAreas();
            AutofacConfig.Register();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
