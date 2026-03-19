using Invoice_Management.Models.ViewModels;
using InvoiceManagement.Application.Interface;
using InvoiceManagement.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Invoice_Management.Controllers
{
    [Authorize]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<ActionResult> Index()
        {
            var products = await _productService.GetAllAsync();

            var vm = products.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                CostPerItem = p.CostPerItem,
                QuantityInStock = p.QuantityInStock
            });

            return View(vm);
        }

        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);

                var vm = new ProductDetailsViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    CostPerItem = product.CostPerItem,
                    QuantityInStock = product.QuantityInStock
                };

                return View(vm);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateProductViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var product = new Product
            {
                Id = 0,
                Name = vm.Name,
                CostPerItem = vm.CostPerItem,
                QuantityInStock = vm.QuantityInStock
            };

            await _productService.CreateAsync(product);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);

                var vm = new EditProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    CostPerItem = product.CostPerItem,
                    QuantityInStock = product.QuantityInStock
                };
                return View(vm);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditProductViewModel productVm)
        {
            if (!ModelState.IsValid) return View(productVm);

            try
            {
                var product = new Product
                {
                    Id = productVm.Id,
                    Name = productVm.Name,
                    CostPerItem = productVm.CostPerItem,
                    QuantityInStock = productVm.QuantityInStock
                };
                await _productService.UpdateAsync(product);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(productVm);
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);

                var vm = new EditProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    CostPerItem = product.CostPerItem,
                    QuantityInStock = product.QuantityInStock
                };

                return View(vm);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _productService.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }
        }
    }
}