using InvoiceManagement.Application.Interface;
using InvoiceManagement.Domain.Entities;
using System;
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
            return View(products);
        }

        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                return View(product);
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
        public async Task<ActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            try
            {
                await _productService.CreateAsync(product);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(product);
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                return View(product);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            try
            {
                await _productService.UpdateAsync(product);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(product);
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                return View(product);
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