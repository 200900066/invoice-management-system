using InvoiceManagement.Infrastructure.Interface;
using InvoiceManagement.Infrastructure.Persistance;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace InvoiceManagement.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UnitOfWork> _logger;

        private DbContextTransaction _transaction;

        public UnitOfWork(
            ApplicationDbContext context,
            IServiceProvider serviceProvider,
            ILogger<UnitOfWork> logger)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public IGenericRepository<TEntity> Repository<TEntity>()
            where TEntity : class
        {
            return _serviceProvider.GetRequiredService<IGenericRepository<TEntity>>();
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _logger.LogInformation("Starting database transaction");
                _transaction = _context.Database.BeginTransaction();
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();

                if (_transaction != null)
                {
                    _transaction.Commit();
                    _logger.LogInformation("Database transaction committed successfully");
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error committing database transaction");
                await RollbackAsync();
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                _logger.LogWarning("Rolling back database transaction");
                _transaction?.Rollback();
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public Task<int> SaveChangesAsync()
        {
            _logger.LogDebug("Saving changes without explicit transaction");
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
