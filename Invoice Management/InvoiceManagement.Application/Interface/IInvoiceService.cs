using InvoiceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceManagement.Application.Interface
{
    public interface IInvoiceService
    {
        Task<Guid> CreateAsync(string userId);

        Task AddItemAsync(Guid invoiceId, int productId, int quantity);

        Task RemoveItemAsync(Guid invoiceId, int productId);

        Task<Invoice> GetByIdAsync(Guid id);

        Task SaveAsync(Guid invoiceId);
    }
}
