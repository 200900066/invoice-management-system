using InvoiceManagement.Application.Interface;
using InvoiceManagement.Domain.Entities;
using InvoiceManagement.Infrastructure.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceManagement.Application.Services
{
    using System.Data.Entity;

    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateAsync(string userId)
        {
            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                CreatedByUserId = userId,
                Total = 0
            };

            await _unitOfWork.Repository<Invoice>().AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();

            return invoice.Id;
        }

        public async Task AddItemAsync(Guid invoiceId, int productId, int quantity)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);

            if (product == null)
                throw new Exception("Product not found");

            if (quantity > product.QuantityInStock)
                throw new Exception("Not enough stock");

            var invoice = await _unitOfWork.Repository<Invoice>()
                .GetSingleAsync(i => i.Id == invoiceId,
                    q => q.Include("Items"));

            if (invoice == null)
                throw new Exception("Invoice not found");

            var existingItem = invoice.Items
                .FirstOrDefault(ii => ii.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var item = new InvoiceItem
                {
                    Id = Guid.NewGuid(),
                    InvoiceId = invoiceId,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.CostPerItem
                };

                invoice.Items.Add(item);
            }

            // Always recalculate from source of truth
            invoice.Total = invoice.Items.Sum(i => i.Quantity * i.UnitPrice);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(Guid invoiceId, int productId)
        {
            var invoice = await _unitOfWork.Repository<Invoice>()
                .GetSingleAsync(i => i.Id == invoiceId,
                    q => q.Include("Items"));

            if (invoice == null)
                throw new Exception("Invoice not found");

            var item = invoice.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
                throw new Exception("Item not found");

            invoice.Items.Remove(item);

            //  Recalculate total
            invoice.Total = invoice.Items.Sum(i => i.Quantity * i.UnitPrice);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Invoice> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.Repository<Invoice>()
                .GetSingleAsync(i => i.Id == id,
                    q => q.Include("Items.Product")); // ✅ EF6 style
        }

        public async Task SaveAsync(Guid invoiceId)
        {
            var invoice = await _unitOfWork.Repository<Invoice>()
                .GetSingleAsync(i => i.Id == invoiceId,
                    q => q.Include("Items"));

            if (invoice == null)
                throw new Exception("Invoice not found");

            foreach (var item in invoice.Items)
            {
                var product = await _unitOfWork.Repository<Product>()
                    .GetByIdAsync(item.ProductId);

                if (product.QuantityInStock < item.Quantity)
                    throw new Exception($"Not enough stock for {product.Name}");

                product.QuantityInStock -= item.Quantity;
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
