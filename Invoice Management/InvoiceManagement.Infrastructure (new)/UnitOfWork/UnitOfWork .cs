using InvoiceManagement.Infrastructure.Interface;
using InvoiceManagement.Infrastructure.Persistance;
using InvoiceManagement.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace InvoiceManagement.Infrastructure.UnitOfWork
{
    
    namespace InvoiceManagement.Infrastructure.UnitOfWork
    {
        public class UnitOfWork : IUnitOfWork
        {
            private readonly ApplicationDbContext _context;

            // Cache repositories
            private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

            private DbContextTransaction _transaction;

            public UnitOfWork(ApplicationDbContext context)
            {
                _context = context;
            }

            // 🔹 Generic repository access
            public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
            {
                var type = typeof(TEntity);

                if (!_repositories.ContainsKey(type))
                {
                    var repo = new GenericRepository<TEntity>(_context);
                    _repositories[type] = repo;
                }

                return (IGenericRepository<TEntity>)_repositories[type];
            }

            // 🔹 Save changes
            public async Task<int> SaveChangesAsync()
            {
                return await _context.SaveChangesAsync();
            }

            // 🔹 Transaction (EF6 style)
            public void BeginTransaction()
            {
                if (_transaction == null)
                {
                    _transaction = _context.Database.BeginTransaction();
                }
            }

            public void Commit()
            {
                try
                {
                    _context.SaveChanges();
                    _transaction?.Commit();
                    _transaction?.Dispose();
                    _transaction = null;
                }
                catch
                {
                    Rollback();
                    throw;
                }
            }

            public void Rollback()
            {
                _transaction?.Rollback();
                _transaction?.Dispose();
                _transaction = null;
            }
        }
    }
}