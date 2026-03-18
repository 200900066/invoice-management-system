using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Invoice_Management.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        
        [Authorize(Roles = "Admin,User,Manager")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            return View();
        }
    }
}