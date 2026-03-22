using InvoiceManagement.Application.Interface;
using InvoiceManagement.Infrastructure.Interface;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceManagement.Application.Services
{
    public class ReportsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;

        public ReportsService()
        {
                
        }

        public async Task<int> GetTotalProducts()
        {
            var res = await _productService.GetAllAsync();
            res.Count();
             return res.Count();
        }
    }
}
