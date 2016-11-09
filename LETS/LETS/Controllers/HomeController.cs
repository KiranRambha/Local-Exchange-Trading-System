using LETS.Helpers;
using LETS.Models;
using System.Web.Mvc;

namespace LETS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public LETSContext DatabaseContext = new LETSContext();

        [AllowAnonymous]
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
        [ValidateAntiForgeryToken]
        public ActionResult ComponentsGuide(RegisterUserViewModel registerUser)
        {
            if (registerUser != null && ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}