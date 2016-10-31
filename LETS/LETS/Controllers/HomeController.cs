using LETS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LETS.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ComponentsGuide()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ComponentsGuide(ComponentsGuideModels componentsGuide)
        {
            if (componentsGuide != null && ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}