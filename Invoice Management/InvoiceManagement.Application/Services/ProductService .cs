using InvoiceManagement.Application.Interface;
using InvoiceManagement.Domain.Entities;
using InvoiceManagement.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvoiceManagement.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(Product product)
        {
            try
            {
                if (product.CostPerItem <= 0)
                    throw new ArgumentException("Invalid price");

                await _unitOfWork.Repository<Product>().AddAsync(product);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating product", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                return await _unitOfWork.Repository<Product>().GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving products", ex);
            }
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            try
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

                return product ?? throw new KeyNotFoundException("Product not found");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving product with id {id}", ex);
            }
        }

        public async Task UpdateAsync(Product product)
        {
            try
            {
                await _unitOfWork.Repository<Product>().Update(product);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating product", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<Product>();

                var product = await repo.GetByIdAsync(id)
                    ?? throw new KeyNotFoundException("Product not found");

                 repo.Delete(product);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting product", ex);
            }
        }
    }
}
