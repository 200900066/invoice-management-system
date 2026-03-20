using InvoiceManagement.Domain.Entities;
using InvoiceManagement.Infrastructure__new_.Repository;
using System;
using System.Threading.Tasks;

namespace InvoiceManagement.Infrastructure.Interface
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

        IInvoiceRepository Invoices { get; }

        Task<int> SaveChangesAsync();

        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
