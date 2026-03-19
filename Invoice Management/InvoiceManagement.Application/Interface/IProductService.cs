using InvoiceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvoiceManagement.Application.Interface
{
    public interface IProductService
    {
        Task CreateAsync(Product product);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(Guid id);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
    }
}
