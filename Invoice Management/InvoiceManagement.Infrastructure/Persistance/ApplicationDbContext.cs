using System.Data.Entity;
using System.Collections.Generic;

namespace InvoiceManagement.Infrastructure.Persistance
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext()
            : base("name=DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}