using AutoMapper;
using Invoice_Management.Models.ViewModels;
using InvoiceManagement.Application.Interface;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Invoice_Management.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ReportsController(IReportService reportService, IMapper mapper, ILogger logger)
        {
            _reportService = reportService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                _logger.Information("Loading reports dashboard");

                var reports = await _reportService.GetReports();
                var vm = _mapper.Map<ReportsViewModel>(reports);

                var warning = await _reportService.GetLowStockNotification();

                if (!string.IsNullOrEmpty(warning))
                {
                    _logger.Warning("Low stock warning triggered: {Warning}", warning);

                    TempData["Warning"] = warning;
                }

                _logger.Information("Reports dashboard loaded successfully");

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading reports dashboard");

                TempData["Error"] = "Something went wrong while loading reports.";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<ActionResult> LowStock()
        {
            try
            {
                _logger.Information("Loading low stock report");

                var reports = await _reportService.GetReports();
                var vm = _mapper.Map<ReportsViewModel>(reports);

                vm.StockVsSoldPerProduct = vm.StockVsSoldPerProduct
                    .Where(x => x.QuantityInStock <= 5)
                    .ToList();

                _logger.Information("Low stock report loaded with {Count} items",
                    vm.StockVsSoldPerProduct.Count);

                if (!vm.StockVsSoldPerProduct.Any())
                {
                    _logger.Warning("Low stock report returned NO items");
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading low stock report");

                TempData["Error"] = "Unable to load low stock items.";
                return RedirectToAction("Index");
            }
        }
    }
}