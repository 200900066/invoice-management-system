using System.Web.Mvc;

namespace Invoice_Management.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ReportsController : Controller
    {
        public ActionResult Dashboard()
        {
            return View();
        }
    }
}