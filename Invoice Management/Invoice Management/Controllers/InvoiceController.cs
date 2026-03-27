using Invoice_Management.Models;
using Invoice_Management.Models.ViewModels;
using Invoice_Management.Models.ViewModels.InvoiceModel;
using InvoiceManagement.Application.Interface;
using InvoiceManagement.Domain.Entities;
using Microsoft.AspNet.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Invoice_Management.Controllers
{
    [Authorize(Roles = "User,Manager")]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IProductService _productService;
        private readonly AutoMapper.IMapper _mapper;
        private readonly ILogger _logger;

        public InvoiceController(
            IInvoiceService invoiceService,
            IProductService productService,
            AutoMapper.IMapper mapper,
            ILogger logger)
        {
            _invoiceService = invoiceService;
            _productService = productService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ActionResult> Index()
        {
            if (User.IsInRole("Manager"))
            {
                var allInvoices = await _invoiceService.ManageInvoice();
                var vm = _mapper.Map<List<InvoiceViewModel>>(allInvoices);
                return View(vm);
            }

            var userName = User.Identity.GetUserName();
            var invoices = await _invoiceService.MyAuthorizedInvoices(userName);
            var vmUser = _mapper.Map<List<InvoiceViewModel>>(invoices);

            return View(vmUser);
        }

        public async Task<ActionResult> Create()
        {
            var userId = User.Identity.GetUserId();
            var createdByUserName = User.Identity.GetUserName();

            var invoiceId = await _invoiceService.CreateAsync(createdByUserName, userId);

            return RedirectToAction("MyInvoiceDetails", new { id = invoiceId });
        }

        [Authorize(Roles = "User,Manager")]
        public async Task<ActionResult> MyInvoiceDetails(Guid id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);

            if (invoice == null)
            {
                TempData["Error"] = "Invoice not found";
                return RedirectToAction("Index");
            }

            if (!User.IsInRole("Manager") && invoice.CreatedByUserName != User.Identity.GetUserName())
            {
                TempData["Error"] = "You are not allowed to view this invoice";
                return RedirectToAction("Index");
            }

            var products = await _productService.GetAllAsync();

            var vm = new InvoiceDataViewModel
            {
                Invoice = _mapper.Map<InvoiceViewModel>(invoice),
                Products = _mapper.Map<List<ProductViewModel>>(products)
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<ActionResult> Finalize(Guid invoiceId, List<InvoiceItemViewModel> vm)
        {
            try
            {
                var userName = User.Identity.GetUserName();

                _logger.Information("User {User} started finalizing invoice {InvoiceId}", userName, invoiceId);

                if (vm == null || vm.Count == 0)
                {
                    _logger.Warning("User {User} is finalizing invoice {InvoiceId} with NO items", userName, invoiceId);
                }

                var dto = _mapper.Map<List<InvoiceItem>>(vm);

                await _invoiceService.FinalizeAsync(invoiceId, dto);

     

                TempData["Success"] = "Invoice saved successfully";
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while finalizing invoice {InvoiceId}", invoiceId);

                TempData["Error"] = "Something went wrong while finalizing invoice";
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Details(Guid id)
        {
            var products = await _productService.GetAllAsync();
            var invoice = await _invoiceService.GetByIdAsync(id);

            var result = new InvoiceDataViewModel
            {
                Invoice = _mapper.Map<InvoiceViewModel>(invoice),
                Products = _mapper.Map<List<ProductViewModel>>(products)
            };

            return View(result);
        }
    }
}