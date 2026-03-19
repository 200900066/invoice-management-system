using InvoiceManagement.Application.Interface;
using InvoiceManagement.Domain.Entities;
using InvoiceManagement.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvoiceManagement.Application.Services
{
    //public class ProductService : IProductService
    //{
    //    private readonly IUnitOfWork _unitOfWork;

    //    public ProductService(IUnitOfWork unitOfWork)
    //    {
    //        _unitOfWork = unitOfWork;
    //    }

    //    public async Task CreateAsync(Product product)
    //    {
    //        if (product.CostPerItem <= 0)
    //            throw new ArgumentException("Invalid price");

    //        await _unitOfWork.Repository<Product>().AddAsync(product);
    //        await _unitOfWork.SaveChangesAsync();
    //    }

    //    public async Task<IEnumerable<Product>> GetAllAsync()
    //    {
    //        return await _unitOfWork.Repository<Product>().GetAllAsync();
    //    }

    //    public async Task<Product> GetByIdAsync(Guid id)
    //    {
    //        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

    //        return product ?? throw new Exception("Product not found");
    //    }

    //    public async Task UpdateAsync(Product product)
    //    {
    //        await _unitOfWork.Repository<Product>().Update(product);
    //        await _unitOfWork.SaveChangesAsync();
    //    }

    //    public async Task DeleteAsync(Guid id)
    //    {
    //        var repo = _unitOfWork.Repository<Product>();
    //        var product = await repo.GetByIdAsync(id) ?? throw new Exception("Product not found");
    //        repo.Delete(product);
    //        await _unitOfWork.SaveChangesAsync();
    //    }
    //}

    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(Product product)
        {
            if (product.CostPerItem <= 0)
                throw new ArgumentException("Invalid price");

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _unitOfWork.Products.GetAllAsync();
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);

            return product ?? throw new Exception("Product not found");
        }

        public async Task UpdateAsync(Product product)
        {
            await _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id)
                ?? throw new Exception("Product not found");

            _unitOfWork.Products.Delete(product);
            await _unitOfWork.SaveAsync();
        }
    }


}
