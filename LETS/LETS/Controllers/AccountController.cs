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
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace LETS.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public readonly LETSContext Context = new LETSContext();

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel loginUser)
        {
            if (loginUser != null && ModelState.IsValid)
            {
                if (true)
                {
                    UserAuthentication userAuthentication = new UserAuthentication();

                    string role = "user";

                    var ident = userAuthentication.AuthenticateUser(loginUser.UserName, role);

                    HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);

                    return RedirectToAction("ComponentsGuide", "Home");
                } else
                {
                    return View();
                }
            }
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Register(RegisterUserViewModel registerUser)
        {
            if (registerUser != null && ModelState.IsValid)
            {
                PasswordHashAndSalt passowordEncription = new PasswordHashAndSalt();
                registerUser.Account.Password = passowordEncription.getHashedPassword(registerUser.Account.Password);
                registerUser.Account.ConfirmPassword = passowordEncription.getHashedPassword(registerUser.Account.ConfirmPassword);
                Context.RegisteredUsers.InsertOne(registerUser);
                return RedirectToAction("RegisteredUsers");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ForgotUsername()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult RegisteredUsers()
        {
            var registeredUsers = Context.RegisteredUsers.Find(new BsonDocument()).ToList();
            return View(registeredUsers);
        }
    }
}