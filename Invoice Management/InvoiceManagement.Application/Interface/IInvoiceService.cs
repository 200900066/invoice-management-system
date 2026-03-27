using InvoiceManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvoiceManagement.Application.Interface
{
    public interface IInvoiceService
    {
        Task<Guid> CreateAsync(string userName, string userId);
        Task<Invoice> GetByIdAsync(Guid id);
        Task<IEnumerable<Invoice>> MyAuthorizedInvoices(string userId);
        Task<Guid> FinalizeAsync(Guid id, List<InvoiceItem> items);
        Task<Invoice> EditInvoiceAsync(Guid id);
        Task<IEnumerable<Invoice>> ManageInvoice();
    }
}
