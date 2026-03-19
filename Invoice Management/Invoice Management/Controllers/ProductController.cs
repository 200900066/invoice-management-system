using InvoiceManagement.Application.Interface;
using InvoiceManagement.Application.Services;
using InvoiceManagement.Infrastructure.Persistance;
using InvoiceManagement.Infrastructure.UnitOfWork;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Invoice_Management.Controllers
{
    [Authorize]
   [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController()
        {
            var context = new ApplicationDbContext();
            var unitOfWork = new UnitOfWork(context);
            _productService = new ProductService(unitOfWork);
        }

        public async Task<ActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            return View(products);
        }
    }

}