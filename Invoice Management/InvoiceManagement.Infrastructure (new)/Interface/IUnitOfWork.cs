using InvoiceManagement.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace InvoiceManagement.Infrastructure.Interface
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync();

        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
