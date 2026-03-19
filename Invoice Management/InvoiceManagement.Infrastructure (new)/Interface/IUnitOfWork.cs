using InvoiceManagement.Domain.Entities;
using System.Threading.Tasks;

namespace InvoiceManagement.Infrastructure.Interface
{
    public interface IUnitOfWork
    {
        IGenericRepository<Product> Products { get; }

        IGenericRepository<Invoice> Invoices { get; }

        IGenericRepository<InvoiceItem> InvoiceItems { get; }

        Task SaveAsync();
    }
}
