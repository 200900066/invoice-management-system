using InvoiceManagement.Application.Interface;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Invoice_Management.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> Index()
        {
            var vm = await _reportService.GetReports();
            return View(vm);
        }
    }
}