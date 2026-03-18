namespace InvoiceManagement.Infrastructure__new_.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<InvoiceManagement.Infrastructure.Persistance.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(InvoiceManagement.Infrastructure.Persistance.ApplicationDbContext context)
        {
          
        }
    }
}
