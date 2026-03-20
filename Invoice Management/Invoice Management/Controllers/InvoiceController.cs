using Invoice_Management.Models;
using Invoice_Management.Models.ViewModels.InvoiceModel;
using InvoiceManagement.Application.Interface;
using InvoiceManagement.Domain.Entities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
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

        public async Task<ActionResult> Index()
        {
            var userName = User.Identity.GetUserName();

            var invoices = await _invoiceService.MyAuthorizedInvoices(userName);

            return View(invoices);
        }

        public async Task<ActionResult> Create()
        {
            var userId = User.Identity.GetUserId();
            var createdByUserName = User.Identity.GetUserName();

            var invoiceId = await _invoiceService.CreateAsync(createdByUserName, userId);

            return RedirectToAction("MyInvoiceDetails", new { id = invoiceId });
        }


        public async Task<ActionResult> MyInvoiceDetails(Guid id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            var product = await _productService.GetAllAsync();

            var result = new InvoiceDataModel
            {
                Invoice = invoice,
                Products = product.ToList()
            };

            return View(result);
        }

        [HttpPost]
        public async Task<ActionResult> Finalize(List<ItemViewModel> vm)
        {


            var dto = vm.Select(i => new InvoiceItem
            {
                InvoiceId = i.InvoiceId,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();

            try
            {
                await _invoiceService.FinalizeAsync(dto);

            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Details(Guid id )
        {
            var product = await _productService.GetAllAsync();
            var editInvoiceItems = await _invoiceService.EditInvoiceAsync(id);

            var result = new InvoiceDataModel
            {
                Invoice = editInvoiceItems,
                Products = product.ToList()
            };

            return View(result);
        }
    }
}
