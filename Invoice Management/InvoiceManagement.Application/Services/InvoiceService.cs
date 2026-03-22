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
        private readonly IProductService _productService;

        public InvoiceService(IUnitOfWork unitOfWork, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
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

        public async Task<IEnumerable<Invoice>> ManageInvoice()
        {
            var invoices = await _unitOfWork.Repository<Invoice>()
                .GetAsync(
                    null,
                    q => q.Include("Items.Product")
                );

            return invoices;
        }

        public async Task<Invoice> EditInvoiceAsync(Guid id)
        {
            return await _unitOfWork.Invoices.GetInvoiceWithItemsAsync(id);
        }

        public async Task<Invoice> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<Invoice>()
                .GetSingleAsync(i => i.Id == id,
                    q => q.Include("Items.Product")); 
        }

        public async Task<IEnumerable<Invoice>> MyAuthorizedInvoices(string userName)
        {
            var invoices = await _unitOfWork.Repository<Invoice>().
                  GetAsync(i => i.CreatedByUserName == userName, p => p.Include("Items"));

            return invoices;
        }


        public async Task<Guid> FinalizeAsync(List<InvoiceItem> items)
        {
            if (items == null || !items.Any())
                throw new ArgumentException("Invoice must have items");
            _unitOfWork.BeginTransaction(); // START TRANSACTION

            try
            {
                var invoiceId = items.FirstOrDefault().InvoiceId;
                var invoice = await GetInvoiceOrThrow(invoiceId); // invoice with items
                var existingItems = invoice.Items.ToList(); // from invoice filter out items

                // now items(new items) processing begin
                await ProcessItemsAsync(invoice, items, existingItems);
                await RemoveDeletedItems(items, existingItems);
                UpdateInvoiceTotal(invoice);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();

                return invoice.Id;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
               
                throw;
            }
        }

        private async Task<Invoice> GetInvoiceOrThrow(Guid invoiceId)
        {
            var invoice = await _unitOfWork.Invoices.GetInvoiceWithItemsAsync(invoiceId)
                ?? throw new KeyNotFoundException($"Invoice {invoiceId} not found");

            return invoice;
        }

        private async Task ProcessItemsAsync(Invoice invoice, List<InvoiceItem> items,
            List<InvoiceItem> existingItems)
        {
            foreach (var item in items)
            {
                var existing = existingItems.FirstOrDefault(x => x.ProductId == item.ProductId);
                var product = await GetProductOrThrow(item.ProductId);
                // STOCK LOGIC
                AdjustStock(product, item, existing);
                if (existing != null) UpdateExistingItem(existing, item);
                else AddNewItem(invoice, item);
            }
        }

        private async Task<Product> GetProductOrThrow(int productId)
        {
            var product = await _productService.GetByIdAsync(productId) ??
                throw new KeyNotFoundException($"Product {productId} not found");

            return product;
        }

        private void UpdateExistingItem(InvoiceItem existing, InvoiceItem item)
        {
            existing.Quantity = item.Quantity;
            existing.UnitPrice = item.UnitPrice;
        }

        private void AddNewItem(Invoice invoice, InvoiceItem item)
        {
            invoice.Items.Add(new InvoiceItem
            {
                Id = Guid.NewGuid(),
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            });
        }

        private void UpdateInvoiceTotal(Invoice invoice)
        {
            invoice.Total = invoice.Items.Sum(i => i.Quantity * i.UnitPrice);
        }

        private void AdjustStock(Product product, InvoiceItem item, InvoiceItem existing)
        {
            var oldQuantity = existing?.Quantity ?? 0;
            var newQuantity = item.Quantity;

            var difference = newQuantity - oldQuantity;

            // Increase consume stock
            if (difference > 0)
            {
                if (product.QuantityInStock < difference) throw new Exception($"Not enough stock for {product.Name}");

                product.QuantityInStock -= difference;
            }
            // Decrease return stock
            else if (difference < 0)
            {
                product.QuantityInStock += difference;
            }
        }

        private async Task RemoveDeletedItems(List<InvoiceItem> items,
            List<InvoiceItem> existingItems)
        {
            foreach (var existing in existingItems)
            {
                // restore products if user update by removing.
                if (!items.Any(i => i.ProductId == existing.ProductId))
                {
                    var product = await GetProductOrThrow(existing.ProductId);
                    product.QuantityInStock += existing.Quantity;
                    var entity = await _unitOfWork.Repository<InvoiceItem>()
                        .GetByIdAsync(existing.Id);
                    _unitOfWork.Repository<InvoiceItem>().Delete(entity);
                }
            }
        }

    }
}