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
            try
            {
                List<InvoiceViewModel> vm;

                if (User.IsInRole("Manager"))
                {
                    var allInvoices = await _invoiceService.ManageInvoice();
                    vm = _mapper.Map<List<InvoiceViewModel>>(allInvoices);
                }
                else
                {
                    var userName = User.Identity.GetUserName();
                    var invoices = await _invoiceService.MyAuthorizedInvoices(userName);
                    vm = _mapper.Map<List<InvoiceViewModel>>(invoices);

                }

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading invoices");

                TempData["Error"] = "Failed to load invoices";

                return View(new List<InvoiceViewModel>());
            }
        }

        public async Task<ActionResult> Create()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var createdByUserName = User.Identity.GetUserName();

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(createdByUserName))
                {
                    TempData["Error"] = "User not authenticated properly";
                    return RedirectToAction("Index");
                }

                var invoiceId = await _invoiceService.CreateAsync(createdByUserName, userId);

                if (invoiceId == Guid.Empty)
                {
                    _logger.Warning($"Invoice creation failed for user {createdByUserName}");

                    TempData["Error"] = "Failed to create invoice";
                    return RedirectToAction("Index");
                }

                TempData["Success"] = "Invoice created successfully";

                return RedirectToAction("MyInvoiceDetails", new { id = invoiceId });
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error creating invoice");

                TempData["Error"] = "Something went wrong while creating invoice";

                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "User,Manager")]
        public async Task<ActionResult> MyInvoiceDetails(Guid id)
        {
            try
            {
                var currentUser = User.Identity.GetUserName();

                var invoice = await _invoiceService.GetByIdAsync(id);

                if (invoice == null)
                {
                    TempData["Error"] = "Invoice not found";
                    return RedirectToAction("Index");
                }

                if (!User.IsInRole("Manager") && invoice.CreatedByUserName != currentUser)
                {
                    _logger.Warning($"Unauthorized access attempt by {currentUser} on invoice {id}");
                    TempData["Error"] = "You are not allowed to view this invoice";
                    return RedirectToAction("Index");
                }

                var products = await _productService.GetAllAsync();

                if (products == null)
                {
                    _logger.Warning("Product list returned null");
                    TempData["Error"] = "Failed to load products";
                    return RedirectToAction("Index");
                }

                var vm = new InvoiceDataViewModel
                {
                    Invoice = _mapper.Map<InvoiceViewModel>(invoice),
                    Products = _mapper.Map<List<ProductViewModel>>(products)
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error loading invoice details for ID: {id}");

                TempData["Error"] = "Something went wrong while loading invoice details";

                return RedirectToAction("Index");
            }
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

        [Authorize(Roles = "User,Manager")]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    TempData["Error"] = "Invalid invoice ID";
                    return RedirectToAction("Index");
                }

                var currentUser = User.Identity.GetUserName();

                var invoice = await _invoiceService.GetByIdAsync(id);

                if (invoice == null)
                {
                    TempData["Error"] = "Invoice not found";
                    return RedirectToAction("Index");
                }

                if (!User.IsInRole("Manager") && invoice.CreatedByUserName != currentUser)
                {
                    _logger.Warning($"Unauthorized access attempt by {currentUser} on invoice {id}");

                    TempData["Error"] = "You are not allowed to view this invoice";
                    return RedirectToAction("Index");
                }

                var products = await _productService.GetAllAsync();

                if (products == null)
                {
                    _logger.Warning("Products returned null");

                    TempData["Error"] = "Failed to load products";
                    return RedirectToAction("Index");
                }

                var result = new InvoiceDataViewModel
                {
                    Invoice = _mapper.Map<InvoiceViewModel>(invoice),
                    Products = _mapper.Map<List<ProductViewModel>>(products)
                };

                return View(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error loading invoice details for ID: {id}");

                TempData["Error"] = "Something went wrong while loading invoice";

                return RedirectToAction("Index");
            }
        }
    }
}