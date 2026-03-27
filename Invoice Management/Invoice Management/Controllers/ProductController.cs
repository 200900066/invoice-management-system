using AutoMapper;
using Invoice_Management.Models.ViewModels;
using InvoiceManagement.Application.Interface;
using InvoiceManagement.Domain.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Invoice_Management.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ProductController(IProductService productService, IMapper mapper, ILogger logger)
        {
            _productService = productService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                _logger.Information("Loading all products");

                var products = await _productService.GetAllAsync();
                var vm = _mapper.Map<List<ProductViewModel>>(products);

                _logger.Information("Loaded {Count} products", vm.Count);

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading products");

                TempData["Error"] = "Unable to load products.";
                return View(new List<ProductViewModel>());
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            try
            {
                _logger.Information("Loading product details for {ProductId}", id);

                var product = await _productService.GetByIdAsync(id);

                if (product == null)
                {
                    _logger.Warning("Product {ProductId} not found", id);

                    TempData["Error"] = "Product not found.";
                    return RedirectToAction("Index");
                }

                var vm = _mapper.Map<ProductViewModel>(product);

                _logger.Information("Product {ProductId} loaded successfully", id);

                TempData["Success"] = "Product loaded successfully.";
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading product {ProductId}", id);

                TempData["Error"] = "Something went wrong while loading product.";
                return RedirectToAction("Index");
            }
        }

        public ActionResult Create()
        {
            _logger.Information("Opening create product page");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateProductViewModel vm)
        {
            try
            {
                _logger.Information("Creating new product");

                if (!ModelState.IsValid)
                {
                    _logger.Warning("Invalid model state while creating product");

                    TempData["Error"] = "Please fix validation errors.";
                    return View(vm);
                }

                var product = _mapper.Map<Product>(vm);

                await _productService.CreateAsync(product);

                _logger.Information("Product created successfully: {ProductName}", product.Name);

                TempData["Success"] = "Product created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating product");

                TempData["Error"] = "Something went wrong while creating product.";
                return View(vm);
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                _logger.Information("Loading product for edit {ProductId}", id);

                var product = await _productService.GetByIdAsync(id);

                if (product == null)
                {
                    _logger.Warning("Product {ProductId} not found for edit", id);

                    TempData["Error"] = "Product not found.";
                    return RedirectToAction("Index");
                }

                var vm = _mapper.Map<EditProductViewModel>(product);

                TempData["Info"] = "Editing product.";

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading product {ProductId} for edit", id);

                TempData["Error"] = "Something went wrong.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditProductViewModel productVm)
        {
            try
            {
                _logger.Information("Updating product {ProductId}", productVm.Id);

                if (!ModelState.IsValid)
                {
                    _logger.Warning("Invalid model state while updating product {ProductId}", productVm.Id);

                    TempData["Error"] = "Please fix validation errors.";
                    return View(productVm);
                }

                var product = _mapper.Map<Product>(productVm);

                await _productService.UpdateAsync(product);

                _logger.Information("Product {ProductId} updated successfully", productVm.Id);

                TempData["Success"] = "Product updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating product {ProductId}", productVm.Id);

                TempData["Error"] = "Something went wrong while updating product.";
                return View(productVm);
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                _logger.Information("Loading product {ProductId} for delete", id);

                var product = await _productService.GetByIdAsync(id);

                if (product == null)
                {
                    _logger.Warning("Product {ProductId} not found for delete", id);

                    TempData["Error"] = "Product not found.";
                    return RedirectToAction("Index");
                }

                var vm = _mapper.Map<EditProductViewModel>(product);

                TempData["Warning"] = "You are about to delete this product.";

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading product {ProductId} for delete", id);

                TempData["Error"] = "Something went wrong.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                _logger.Information("Deleting product {ProductId}", id);

                await _productService.DeleteAsync(id);

                _logger.Information("Product {ProductId} deleted successfully", id);

                TempData["Success"] = "Product deleted successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error deleting product {ProductId}", id);

                TempData["Error"] = "Unable to delete product.";
                return RedirectToAction("Delete", new { id });
            }
        }
    }
}