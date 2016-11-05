using System.Linq;
using System.Web;
using System.Web.Mvc;
using LETS.Models;
using MongoDB.Driver;
using LETS.Helpers;
using MongoDB.Bson;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

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
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel loginUser)
        {
            if (loginUser != null && ModelState.IsValid)
            {
                var user = await Context.RegisteredUsers.Find(new BsonDocument {
                    { "Account.UserName", loginUser.UserName }
                }).ToListAsync();

                PasswordHashAndSalt passowordEncription = new PasswordHashAndSalt();
                loginUser.Password = passowordEncription.getHashedPassword(loginUser.Password);

                if (user.Count > 0)
                {
                    if (user[0].Account.UserName.Equals(loginUser.UserName) && user[0].Account.Password.Equals(loginUser.Password))
                    {
                        UserAuthentication userAuthentication = new UserAuthentication();

                        string role = "admin";

                        var ident = userAuthentication.AuthenticateUser(loginUser.UserName, role);

                        HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);

                        return RedirectToAction("ComponentsGuide", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "Please make sure you entered the correct username.");
                        ModelState.AddModelError("Password", "Please make sure you entered the correct password.");
                        View();
                    }
                }
                else
                {
                    ModelState.AddModelError("UserName", "Please make sure you entered the correct username.");
                    ModelState.AddModelError("Password", "Please make sure you entered the correct password.");
                    return View();
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Logoff()
        {
            var AutheticationManager = HttpContext.GetOwinContext().Authentication;
            AutheticationManager.SignOut();

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterUserViewModel registerUser)
        {
            ReCaptcha recaptcha = new ReCaptcha();
            string responseFromServer = recaptcha.OnActionExecuting();
            if (responseFromServer.StartsWith("true"))
            {
                if (registerUser != null && ModelState.IsValid)
                {
                    var userByUsername = await Context.RegisteredUsers.Find(new BsonDocument {
                    { "Account.UserName", registerUser.Account.UserName }
                }).ToListAsync();

                    var userByEmail = await Context.RegisteredUsers.Find(new BsonDocument {
                    { "Account.Email", registerUser.Account.Email }
                }).ToListAsync();

                    if (userByUsername.Count == 0)
                    {
                        if (userByEmail.Count == 0)
                        {
                            PasswordHashAndSalt passowordEncription = new PasswordHashAndSalt();
                            registerUser.Account.Password = passowordEncription.getHashedPassword(registerUser.Account.Password);
                            registerUser.Account.ConfirmPassword = passowordEncription.getHashedPassword(registerUser.Account.ConfirmPassword);
                            Context.RegisteredUsers.InsertOne(registerUser);
                            return RedirectToAction("RegisteredUsers");
                        }
                        else
                        {
                            registerUser.Account.Password = null;
                            registerUser.Account.ConfirmPassword = null;
                            ModelState.AddModelError("Account.Email", "Sorry, The following email already exists in our system.");
                            return View(registerUser);
                        }
                    }
                    else
                    {
                        registerUser.Account.Password = null;
                        registerUser.Account.ConfirmPassword = null;
                        ModelState.AddModelError("Account.UserName", "Sorry, This username is not available.");

                        if (userByEmail.Count > 0)
                        {
                            ModelState.AddModelError("Account.Email", "Sorry, The following email already exists in our system.");
                        }

                        return View(registerUser);
                    }
                }
            }
            else
            {
                registerUser.Account.Password = null;
                registerUser.Account.ConfirmPassword = null;
                ModelState.AddModelError("ReCaptcha", "Incorrect CAPTCHA entered.");
                return View(registerUser);
            }
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ForgotUsername()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel forgotPassword)
        {
            if (forgotPassword != null && ModelState.IsValid)
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress("rhulletsteam@gmail.com");
                    mail.To.Add(forgotPassword.Email);
                    mail.Subject = "Royal Holloway LETS Password Recovery";
                    mail.Body = "<h1>Please enter the below text along with your username when loggin in</h1><br/><p><b>ABCDEFGH</b><p>";
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("rhulletsteam@gmail.com", "zyvb492@rhul@egham");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
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