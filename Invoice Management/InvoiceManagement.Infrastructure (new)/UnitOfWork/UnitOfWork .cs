using InvoiceManagement.Domain.Entities;
using InvoiceManagement.Infrastructure.Interface;
using InvoiceManagement.Infrastructure.Persistance;
using InvoiceManagement.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace InvoiceManagement.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IGenericRepository<Product> Products { get; }
        public IGenericRepository<Invoice> Invoices { get; }
        public IGenericRepository<InvoiceItem> InvoiceItems { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Products = new GenericRepository<Product>(_context);
            Invoices = new GenericRepository<Invoice>(_context);
            InvoiceItems = new GenericRepository<InvoiceItem>(_context);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
