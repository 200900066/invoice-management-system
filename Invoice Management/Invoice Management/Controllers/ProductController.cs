using AutoMapper;
using Invoice_Management.Models.ViewModels;
using InvoiceManagement.Application.Interface;
using InvoiceManagement.Domain.Entities;
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
        private readonly AutoMapper.IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                var products = await _productService.GetAllAsync();

                var vm = _mapper.Map<List<ProductViewModel>>(products);

                return View(vm);
            }
            catch (Exception ex)
            {

                TempData["Error"] = "Unable to load products.";

                return View(new List<ProductViewModel>());
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);

                if (product == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction("Index");
                }

                var vm = _mapper.Map<ProductViewModel>(product);

                TempData["Success"] = "Product loaded successfully.";

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Something went wrong while loading product.";

                return RedirectToAction("Index");
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
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Please fix validation errors.";
                    return View(vm);
                }

                var product = _mapper.Map<Product>(vm);

                await _productService.CreateAsync(product);

                TempData["Success"] = "Product created successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                TempData["Error"] = "Something went wrong while creating product.";

                return View(vm);
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);

                if (product == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction("Index");
                }

                var vm = _mapper.Map<EditProductViewModel>(product);

                TempData["Info"] = "Editing product.";

                return View(vm);
            }
            catch (Exception ex)
            {

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
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Please fix validation errors.";
                    return View(productVm);
                }

                var product = _mapper.Map<Product>(productVm);

                await _productService.UpdateAsync(product);

                TempData["Success"] = "Product updated successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                TempData["Error"] = "Something went wrong while updating product.";

                return View(productVm);
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);

                if (product == null)
                {
                    TempData["Error"] = "Product not found.";
                    return RedirectToAction("Index");
                }

                var vm = _mapper.Map<EditProductViewModel>(product);

                TempData["Warning"] = "You are about to delete this product.";

                return View(vm);
            }
            catch (Exception ex)
            {
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
                await _productService.DeleteAsync(id);

                TempData["Success"] = "Product deleted successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Unable to delete product.";

                return RedirectToAction("Delete", new { id });
            }
        }
    }
}