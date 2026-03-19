using Invoice_Management.Models.ViewModels.InvoiceModel;
using InvoiceManagement.Application.Interface;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Invoice_Management.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IProductService _productService;

        public InvoiceController(IInvoiceService invoiceService, IProductService productService)
        {
            _invoiceService = invoiceService;
            _productService = productService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Create()
        {
            var userId = User.Identity.GetUserId();

            var createdByUserName = User.Identity.GetUserName();

            var invoiceId = await _invoiceService.CreateAsync(userId);

            return RedirectToAction("Details", new { id = invoiceId });
        }

        [HttpPost]
        public async Task<ActionResult> AddItem(Guid invoiceId, int productId, int quantity)
        {
            try
            {
                await _invoiceService.AddItemAsync(invoiceId, productId, quantity);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", new { id = invoiceId });
        }

        [HttpPost]
        public async Task<ActionResult> RemoveItem(Guid invoiceId, int productId)
        {
            try
            {
                await _invoiceService.RemoveItemAsync(invoiceId, productId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", new { id = invoiceId });
        }

        [HttpPost]
        public async Task<ActionResult> Save(Guid invoiceId)
        {
            try
            {
                await _invoiceService.SaveAsync(invoiceId);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", new { id = invoiceId });
        }

        public async Task<ActionResult> Details(Guid? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var invoice = await _invoiceService.GetByIdAsync(id.Value);

            if (invoice == null)
                return HttpNotFound();

            var products = await _productService.GetAllAsync();

            var vm = new InvoiceDetailsViewModel
            {
                Id = invoice.Id,
                Total = invoice.Total,
                Items = invoice.Items.Select(i => new InvoiceItemViewModel
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList(),

                Products = products.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
            };

            return View(vm);
        }
    }
}