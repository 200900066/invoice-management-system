using InvoiceManagement.Application.Services;
using System.Threading.Tasks;

namespace InvoiceManagement.Application.Interface
{
    public interface IReportService
    {
        Task<ReportData> GetReports();
    }
}
