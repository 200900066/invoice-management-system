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

        //public async Task<Guid> FinalizeAsync(List<InvoiceItem> items)
        //{
        //    if (items == null || !items.Any())
        //        throw new Exception("Invoice must have items");

        //    var invoiceId = items.First().InvoiceId;

        //    var invoice = await _unitOfWork.Invoices
        //        .GetInvoiceWithItemsAsync(invoiceId);

        //    var existingItems = invoice.Items.ToList();

        //    foreach (var item in items)
        //    {
        //        var product = await _unitOfWork.Repository<Product>()
        //            .GetByIdAsync(item.ProductId);

        //        var existing = existingItems
        //            .FirstOrDefault(x => x.Id == item.Id);

        //        if (existing != null)
        //        {
        //            // UPDATE EXISTING ITEM

        //            var quantityDifference = item.Quantity - existing.Quantity;

        //            // Only validate if increasing quantity
        //            if (quantityDifference > 0)
        //            {
        //                if (product.QuantityInStock < quantityDifference)
        //                    throw new Exception($"Not enough stock for {product.Name}");

        //                product.QuantityInStock -= quantityDifference;
        //            }
        //            else if (quantityDifference < 0)
        //            {
        //                // Returning stock if reduced
        //                product.QuantityInStock += Math.Abs(quantityDifference);
        //            }

        //            existing.Quantity = item.Quantity;
        //            existing.UnitPrice = item.UnitPrice;
        //        }
        //        else
        //        {
        //            // NEW ITEM

        //            if (product.QuantityInStock < item.Quantity)
        //                throw new Exception($"Not enough stock for {product.Name}");

        //            product.QuantityInStock -= item.Quantity;

        //            invoice.Items.Add(new InvoiceItem
        //            {
        //                Id = Guid.NewGuid(),
        //                ProductId = item.ProductId,
        //                Quantity = item.Quantity,
        //                UnitPrice = item.UnitPrice
        //            });
        //        }
        //    }

        //    // HANDLE DELETED ITEMS (RETURN STOCK)
        //    foreach (var existing in existingItems)
        //    {
        //        if (!items.Any(i => i.Id == existing.Id))
        //        {
        //            var product = await _unitOfWork.Repository<Product>()
        //                .GetByIdAsync(existing.ProductId);

        //            product.QuantityInStock += existing.Quantity;

        //            _unitOfWork.Repository<InvoiceItem>().Delete(existing);
        //        }
        //    }

        //    // RECALCULATE TOTAL
        //    invoice.Total = invoice.Items.Sum(i => i.Quantity * i.UnitPrice);

        //    await _unitOfWork.SaveChangesAsync();

        //    return invoice.Id;
        //}

        public async Task<Guid> FinalizeAsync(List<InvoiceItem> items)
        {
            if (items == null || !items.Any())
                throw new Exception("Invoice must have items");
            var invoiceId = items.First().InvoiceId;

            var invoice = await _unitOfWork.Invoices
                .GetInvoiceWithItemsAsync(invoiceId);

            // EXISTING ITEMS FROM DB
            var existingItems = invoice.Items.ToList();

            foreach (var item in items)
            {
                var existing = existingItems
                    .FirstOrDefault(x => x.ProductId == item.ProductId);

                var product = await _unitOfWork.Repository<Product>()
                  .GetByIdAsync(item.ProductId);

                if (product.QuantityInStock < item.Quantity)
                    throw new Exception($"Not enough stock for {product.Name}");

                product.QuantityInStock -= item.Quantity;


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