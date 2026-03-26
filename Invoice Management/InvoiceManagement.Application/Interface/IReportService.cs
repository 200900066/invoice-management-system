using InvoiceManagement.Application.Services;
using InvoiceManagement.Domain.Entities;
using System.Threading.Tasks;

namespace InvoiceManagement.Application.Interface
{
    public interface IReportService
    {
        Task<Report> GetReports();
        Task<string> GetLowStockNotification();
    }
}
