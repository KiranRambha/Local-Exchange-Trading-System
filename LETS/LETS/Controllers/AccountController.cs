using System.Linq;
using System.Web;
using System.Web.Mvc;
using LETS.Models;
using MongoDB.Driver;
using LETS.Helpers;
using MongoDB.Bson;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System;
using System.Collections.Generic;

namespace LETS.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public readonly LETSContext DatabaseContext = new LETSContext();
        private static readonly Random Random = new Random();

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
                var userByUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.UserName", loginUser.UserName }
                }).ToListAsync();

                var passowordEncryption = new PasswordHashAndSalt();
                loginUser.Password = passowordEncryption.getHashedPassword(loginUser.Password);

                if (userByUsername.Count > 0)
                {
                    if (userByUsername[0].Account.UserName.Equals(loginUser.UserName) && (userByUsername[0].Account.Password.Equals(loginUser.Password) || (!string.IsNullOrEmpty(userByUsername[0].Account.TempPassword) && userByUsername[0].Account.TempPassword.Equals(loginUser.Password))))
                    {
                        var userAuthentication = new UserAuthentication();

                        var role = "admin";
                        var identity = userAuthentication.AuthenticateUser(loginUser.UserName, role);
                        HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
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
            var autheticationManager = HttpContext.GetOwinContext().Authentication;
            autheticationManager.SignOut();

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
            var recaptcha = new ReCaptcha();
            var responseFromServer = recaptcha.OnActionExecuting();
            if (responseFromServer.StartsWith("true"))
            {
                if (registerUser != null && ModelState.IsValid)
                {
                    var userByUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.UserName", registerUser.Account.UserName }
                }).ToListAsync();

                    var userByEmail = await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.Email", registerUser.Account.Email }
                }).ToListAsync();

                    if (userByUsername.Count == 0)
                    {
                        if (userByEmail.Count == 0)
                        {
                            var passwordEncryption = new PasswordHashAndSalt();
                            registerUser.Account.Password = passwordEncryption.getHashedPassword(registerUser.Account.Password);
                            registerUser.Account.ConfirmPassword = passwordEncryption.getHashedPassword(registerUser.Account.ConfirmPassword);
                            DatabaseContext.RegisteredUsers.InsertOne(registerUser);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotUsername(ForgotUsernameViewModel forgotUsername)
        {
            var recaptcha = new ReCaptcha();
            var responseFromServer = recaptcha.OnActionExecuting();
            if (responseFromServer.StartsWith("true"))
            {
                if (forgotUsername != null && ModelState.IsValid)
                {
                    var userByEmail = await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                        { "Account.Email", forgotUsername.Email }
                    }).ToListAsync();

                    if (userByEmail.Count > 0)
                    {
                        using (var mail = new MailMessage())
                        {
                            mail.To.Add(forgotUsername.Email);
                            mail.Subject = "Royal Holloway LETS Username Recovery";
                            mail.Body = "<p>Hello " + userByEmail[0].About.FirstName + ",</p><h3>Forgotten your username?</h3><p>We got a request about your Royal Holloway LETS account's username.<br/>Please find your username highlighted in bold below.<br/></p><h2>" + userByEmail[0].Account.UserName + "</h2><p>All the best,<br/>Royal Holloway LETS</p>";
                            SendEmail(mail);
                            ModelState.AddModelError("Success", "Please check you email, We have sent you your username.");
                            forgotUsername.Email = null;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Email", "Sorry, The Email you provided is not present in our system.");
                        return View(forgotUsername);
                    }
                }
            }
            else
            {
                ModelState.AddModelError("ReCaptcha", "Incorrect CAPTCHA entered.");
                return View(forgotUsername);
            }
            return View();
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
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel forgotPassword)
        {
            var recaptcha = new ReCaptcha();
            var responseFromServer = recaptcha.OnActionExecuting();
            if (responseFromServer.StartsWith("true"))
            {
                if (forgotPassword != null && ModelState.IsValid)
                {
                    var userByUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                        { "Account.UserName", forgotPassword.UserName }
                    }).ToListAsync();

                    if (userByUsername.Count > 0)
                    {
                        if (userByUsername[0].Account.Email.Equals(forgotPassword.Email))
                        {
                            var password = CreatePassword();
                            var passwordEncryption = new PasswordHashAndSalt();
                            var tempEncryptedPassword = passwordEncryption.getHashedPassword(password);
                            userByUsername[0].Account.TempPassword = tempEncryptedPassword;
                            await DatabaseContext.RegisteredUsers.ReplaceOneAsync(r => r.Account.UserName == userByUsername[0].Account.UserName, userByUsername[0]);
                            using (var mail = new MailMessage())
                            {
                                mail.To.Add(forgotPassword.Email);
                                mail.Subject = "Royal Holloway LETS Password Recovery";
                                mail.Body = "<p>Hello " + userByUsername[0].About.FirstName + ",</p><h3>Forgotten your password?</h3><p>We got a request to reset your Royal Holloway LETS account's password.<br/>You use the below code in bold to login to your account.<br/><b>Please change your password to something memorable when you have logged in.</b></p><h2>" + password + "</h2><p>All the best,<br/>Royal Holloway LETS</p>";
                                SendEmail(mail);
                                ModelState.AddModelError("Success", "Please check you email, We have sent you your recovery password to your account.");
                                forgotPassword.UserName = null;
                                forgotPassword.Email = null;
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Email", "Sorry, The Email you provided is not associated with the username you entered.");
                            return View(forgotPassword);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "Sorry, We didn't find any account associated with this username in our system.");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("ReCaptcha", "Incorrect CAPTCHA entered.");
                return View(forgotPassword);
            }
            return View();
        }

        public string CreatePassword()
        {
            var randomPassword = RandomCharacter;
            return randomPassword;
        }

        public string RandomCharacter
        {
            get
            {
                const string valid = "abcdFGHIJKLefghijklm678STUVW90nopqrstBCDEMNOuvwxyz12345APQRXYZ";
                return new string(Enumerable.Repeat(valid, 16).Select(s => s[Random.Next(s.Length)]).ToArray());
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult RegisteredUsers()
        {
            var registeredUsers = DatabaseContext.RegisteredUsers.Find(new BsonDocument()).ToList();
            return View(registeredUsers);
        }

        public async Task<List<RegisterUserViewModel>> GetUserByUsername(string userName)
        {
            return await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.UserName", userName }
                }).ToListAsync();
        }

        public ActionResult UserProfile()
        {
            return View();
        }

        public void SendEmail(MailMessage mail)
        {
            mail.From = new MailAddress("rhulletsteam@gmail.com");
            mail.IsBodyHtml = true;

            using (var smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.Credentials = new NetworkCredential("rhulletsteam@gmail.com", "zyvb492@rhul@egham");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
        }
    }
}