using InvoiceManagement.Domain.Entities;
using InvoiceManagement.Infrastructure.Persistance;
using InvoiceManagement.Infrastructure.Repository;

namespace InvoiceManagement.Infrastructure__new_.Repository
{
    public class ProductRepository : GenericRepository<Product>
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
