using AutoMapper;
using Invoice_Management.Models.ViewModels;
using InvoiceManagement.Application.Interface;
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
        public ReportsController(IReportService reportService, IMapper mapper)
        {
            _reportService = reportService;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                var reports = await _reportService.GetReports();

                var vm = _mapper.Map<ReportsViewModel>(reports);

                var warning = await _reportService.GetLowStockNotification();

                if (!string.IsNullOrEmpty(warning))
                {
                    TempData["Warning"] = warning;
                }

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Something went wrong while loading reports.";

                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<ActionResult> LowStock()
        {
            try
            {
                var reports = await _reportService.GetReports();

                var vm = _mapper.Map<ReportsViewModel>(reports);

                // filter only low stock items
                vm.StockVsSoldPerProduct = vm.StockVsSoldPerProduct
                    .Where(x => x.QuantityInStock <= 5)
                    .ToList();

                return View(vm);
            }
            catch (Exception ex)
            {

                TempData["Error"] = "Unable to load low stock items.";

                return RedirectToAction("Index");
            }
        }
    }
}