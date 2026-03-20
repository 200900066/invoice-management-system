using InvoiceManagement.Domain.Entities;
using InvoiceManagement.Infrastructure.Persistance;
using InvoiceManagement.Infrastructure.Repository;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceManagement.Infrastructure__new_.Repository
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Invoice> GetInvoiceWithItemsAsync(Guid invoiceId)
        {
            return await _dbSet.Include("Items.Product").FirstOrDefaultAsync(i => i.Id == invoiceId);
        }
    }
}
