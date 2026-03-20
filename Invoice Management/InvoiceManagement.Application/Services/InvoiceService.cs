using InvoiceManagement.Application.Interface;
using InvoiceManagement.Domain.Entities;
using InvoiceManagement.Infrastructure.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Entity;

namespace InvoiceManagement.Application.Services
{

    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateAsync(string userName, string userId)
        {
            var invoice = new Invoice
            {
                CreatedByUserName = userName,
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                CreatedByUserId = userId,
                Total = 0
            };

            await _unitOfWork.Repository<Invoice>().AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();

            return invoice.Id;
        }

        public async Task<Invoice> EditInvoiceAsync(Guid id)
        {
            return await _unitOfWork.Invoices.GetInvoiceWithItemsAsync(id);
        }

        public async Task<Invoice> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<Invoice>()
                .GetSingleAsync(i => i.Id == id,
                    q => q.Include("Items.Product")); // ✅ EF6 style
        }

        /*   public async Task<Guid> FinalizeAsync(List<InvoiceItem> items)
           {
               if (items == null || !items.Any())
                   throw new Exception("Invoice must have items");

               var invoice = await _unitOfWork.Repository<Invoice>()
                   .GetByIdAsync(items[0].InvoiceId);

               foreach (var item in items)
               {
                   var product = await _unitOfWork.Repository<Product>()
                       .GetByIdAsync(item.ProductId);

                   if (product.QuantityInStock < item.Quantity)
                       throw new Exception($"Not enough stock for {product.Name}");

                   product.QuantityInStock -= item.Quantity;

                   await _unitOfWork.Repository<InvoiceItem>().AddAsync(new InvoiceItem
                   {
                       Id = Guid.NewGuid(),
                       ProductId = item.ProductId,
                       InvoiceId = item.InvoiceId,
                       Quantity = item.Quantity,
                       UnitPrice = item.UnitPrice
                   });
               }

               invoice.Total = items.Sum(i => i.Quantity * i.UnitPrice);

               await _unitOfWork.Repository<Invoice>().Update(invoice);
               await _unitOfWork.SaveChangesAsync();

               return invoice.Id;
           }*/

        public async Task<Guid> FinalizeAsync(List<InvoiceItem> items)
        {
            var invoiceId = items.First().InvoiceId;

            var invoice = await _unitOfWork.Invoices
                .GetInvoiceWithItemsAsync(invoiceId);

            // EXISTING ITEMS FROM DB
            var existingItems = invoice.Items.ToList();

            foreach (var item in items)
            {
                var existing = existingItems
                    .FirstOrDefault(x => x.Id == item.Id);

                if (existing != null)
                {
                    // UPDATE
                    existing.Quantity = item.Quantity;
                    existing.UnitPrice = item.UnitPrice;
                }
                else
                {
                    //  NEW ITEM
                    invoice.Items.Add(new InvoiceItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    });
                }
            }

            // REMOVE DELETED ITEMS
            foreach (var existing in existingItems)
            {
                if (!items.Any(i => i.Id == existing.Id))
                {
                    _unitOfWork.Repository<InvoiceItem>().Delete(existing);
                }
            }

            invoice.Total = invoice.Items.Sum(i => i.Quantity * i.UnitPrice);

            await _unitOfWork.SaveChangesAsync();

            return invoice.Id;
        }

        public async Task<IEnumerable<Invoice>> MyAuthorizedInvoices(string userName)
        {
            var invoices = await _unitOfWork.Repository<Invoice>().
                  GetAsync(i => i.CreatedByUserName == userName, p => p.Include("Items"));

            return invoices;
        }
    }
}