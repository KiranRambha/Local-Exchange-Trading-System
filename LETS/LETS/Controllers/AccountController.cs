using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LETS.Models;
using MongoDB.Driver;
using LETS.Properties;
using LETS.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace LETS.Controllers
{
    public class AccountController : Controller
    {
        public LETSContext Context = new LETSContext();

        public AccountController()
        {
            JsonWriterSettings.Defaults.Indent = true;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterUserViewModel registerUser)
        {
            if (registerUser != null && ModelState.IsValid)
            {
                registerUser.PersonId = Guid.NewGuid().ToString();
                var person = registerUser.ToJson();
                System.Diagnostics.Debug.WriteLine(person);
                return View();
            }
            return View();
        }

        [HttpGet]
        public ActionResult ForgotUsername()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
    }
}