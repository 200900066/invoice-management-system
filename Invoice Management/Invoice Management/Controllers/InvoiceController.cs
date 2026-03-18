using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Invoice_Management.Controllers
{

    [Authorize]
    public class InvoiceController : Controller
    {
       
        public ActionResult Index()
        {
            return View();
        }
    }
}