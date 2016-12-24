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
using static System.Configuration.ConfigurationManager;

namespace LETS.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public readonly LETSContext DatabaseContext = new LETSContext(); 
        private static readonly Random Random = new Random();

        /// <summary>
        /// Get method for the Login Page
        /// </summary>
        /// <returns>returns Login view if the user is not authenticated, else takes the user to the index page</returns>
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

        /// <summary>
        /// Post method for the Login Page
        /// </summary>
        /// <param name="loginUser">Holds the entered user credentials i.e. Username and Password</param>
        /// <returns>Verifies the user credentials, takes them to the user profile page if valid else takes them back to the login page.</returns>
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
                        var identity = userAuthentication.AuthenticateUser(userByUsername[0].Account.UserName);
                        HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
                        return RedirectToAction("UserProfile", "Account");
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

        /// <summary>
        /// Get method for the Logoff page.
        /// </summary>
        /// <returns>Logs off the user, kills the session and redirects the user to the index page.</returns>
        [HttpGet]
        public ActionResult Logoff()
        {
            var autheticationManager = HttpContext.GetOwinContext().Authentication;
            autheticationManager.SignOut();
            if (Session != null)
            {
                Session.Clear();
                Session.Abandon();
            }
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Get method for the Register Page
        /// </summary>
        /// <returns>returns the Register page if the user is not authenticated else takes the user to the index page.</returns>
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

        /// <summary>
        /// Post method for the Register Page
        /// </summary>
        /// <param name="registerUser">Holds the data entered by the user on the registration page.</param>
        /// <returns>saves the data to the database, sends an email and reloads the page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Register(RegisterUserViewModel registerUser)
        {
            var recaptcha = new ReCaptcha();
            var responseFromServer = recaptcha.OnActionExecuting();
            if (responseFromServer.StartsWith("true", StringComparison.Ordinal))
            {
                if (registerUser != null && ModelState.IsValid)
                {
                    var userByUsername = DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.UserName", registerUser.Account.UserName }
                }).ToList();

                    var userByEmail = DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.Email", registerUser.Account.Email }
                }).ToList();

                    if (userByUsername.Count == 0)
                    {
                        if (userByEmail.Count == 0)
                        {
                            var passwordEncryption = new PasswordHashAndSalt();
                            registerUser.Id = Guid.NewGuid().ToString();
                            registerUser.Account.Password = passwordEncryption.getHashedPassword(registerUser.Account.Password);
                            registerUser.Account.ConfirmPassword = passwordEncryption.getHashedPassword(registerUser.Account.ConfirmPassword);
                            var tradingDetails = new LetsTradingDetails { Id = registerUser.Id, Credit = 100 };
                            DatabaseContext.RegisteredUsers.InsertOne(registerUser);
                            DatabaseContext.LetsTradingDetails.InsertOne(tradingDetails);

                            using (var mail = new MailMessage())
                            {
                                mail.To.Add(registerUser.Account.Email);
                                mail.Subject = "Welcome to Royal Holloway LETS";
                                mail.Body = "<p>Hello " + registerUser.About.FirstName + ",</p><h3>Thanks for joining Royal Holloway LETS</h3><p>Please find your account details below</p><p>Title : <b>" + registerUser.About.Title + "</b></p><p>First Name : <b>" + registerUser.About.FirstName + "</b></p><p>Last Name : <b>" + registerUser.About.LastName + "</b></p><p>Gender : <b>" + registerUser.About.Gender + "</b></p><p>User Name : <b>" + registerUser.Account.UserName + "</b></p><p>Kind Regards,<br/>Royal Holloway LETS</p>";
                                SendEmail(mail);
                                TempData.Add("Registered", "You have successfully signed up for Royal Holloway LETS, We have also sent you can email with your account details for your future reference.");
                            }

                            return RedirectToAction("Login");
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

        /// <summary>
        /// Checks if a username is present in the database or not
        /// </summary>
        /// <param name="userName">represents the username entered by the user</param>
        /// <returns>returns true or false depending on if the username is present in the database or not</returns>
        [AllowAnonymous]
        public bool CheckUserName(string userName)
        {
            var userByUsername = DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                        { "Account.UserName", userName }
                    }).ToList();

            return userByUsername.Count == 0;
        }

        /// <summary>
        /// Get Method for the ForgotUsername page.
        /// </summary>
        /// <returns>returns the forgot username page if the user is not authenticated else takes the user to the index page.</returns>
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

        /// <summary>
        /// Post method for the forgot username
        /// </summary>
        /// <param name="forgotUsername">Holds the details entered by the user on the forgot username page.</param>
        /// <returns>sends an email to the user and returns the user to the login page.</returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotUsername(ForgotUsernameViewModel forgotUsername)
        {
            var recaptcha = new ReCaptcha();
            var responseFromServer = recaptcha.OnActionExecuting();
            if (responseFromServer.StartsWith("true", StringComparison.Ordinal))
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

        /// <summary>
        /// Get method for the forgotpassword page
        /// </summary>
        /// <returns>returns the forgotpassword if the user is not authenticated else takes the user to the index page.</returns>
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

        /// <summary>
        /// Post method for the forgotpassword page
        /// </summary>
        /// <param name="forgotPassword">Stores the data entered by the user on the page</param>
        /// <returns>sends an email to the user and takes the user to the login page.</returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel forgotPassword)
        {
            var recaptcha = new ReCaptcha();
            var responseFromServer = recaptcha.OnActionExecuting();
            if (responseFromServer.StartsWith("true", StringComparison.Ordinal))
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

        /// <summary>
        /// Creates a random password when the user clicks and submit forgot password.
        /// </summary>
        /// <returns>returns a random string as a password.</returns>
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

        /// <summary>
        /// Get method for the RegisteredUsers Page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult RegisteredUsers()
        {
            var registeredUsers = DatabaseContext.RegisteredUsers.Find(new BsonDocument()).ToList();
            return View(registeredUsers);
        }

        /// <summary>
        /// Get method for the UserProfile page.
        /// </summary>
        /// <returns>returns the user profile page is the user is authenticated.</returns>
        [HttpGet]
        public async Task<ActionResult> UserProfile()
        {
            var username = User.Identity.Name;
            var userByUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.UserName", username }
                }).ToListAsync();

            var userTradingDetails = await DatabaseContext.LetsTradingDetails.Find(new BsonDocument {
                    { "_id", userByUsername[0].Id }
                }).ToListAsync();

            var letsUser = new LetsUser
            {
                UserPersonalDetails = userByUsername[0],
                UserTradingDetails = userTradingDetails[0]
            };

            return View(letsUser);
        }

        /// <summary>
        /// Sends an email to the email address provided
        /// </summary>
        /// <param name="mail">Contains the data of the email like the body and the mailTo: address</param>
        public void SendEmail(MailMessage mail)
        {
            mail.From = new MailAddress("rhulletsteam@gmail.com");
            mail.IsBodyHtml = true;

            using (var smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                var email = AppSettings.GetValues("RHLETS.Email").FirstOrDefault();
                var password = AppSettings.GetValues("RHLETS.Password").FirstOrDefault();
                smtp.Credentials = new NetworkCredential(email, password);
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
        }

        /// <summary>
        /// Post method for the account settings partial
        /// </summary>
        /// <param name="registeredUser">stores the data entered by the user on the account settings partial</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AccountSettingsEdit(RegisterUserViewModel registeredUser)
        {
            var userName = User.Identity.Name;

            var userByUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.UserName", userName }
                }).ToListAsync();

            var userByEmail = await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.Email", registeredUser.Account.Email }
                }).ToListAsync();

            registeredUser.Id = userByUsername[0].Id;
            registeredUser.Account.UserName = userByUsername[0].Account.UserName;
            registeredUser.Account.Password = userByUsername[0].Account.Password;
            registeredUser.Account.ConfirmPassword = userByUsername[0].Account.Password;
            ModelState.Clear();
            TryValidateModel(registeredUser);

            if (ModelState.IsValid)
            {
                if (userByEmail.Count <= 1)
                {
                    await DatabaseContext.RegisteredUsers.ReplaceOneAsync(r => r.Account.UserName == registeredUser.Account.UserName, registeredUser);
                }
            }
            return RedirectToAction("UserProfile", "Account");
        }

        /// <summary>
        /// Get Method for the account settings partial
        /// </summary>
        /// <returns>returns the account settings partial when the user clicks edit on the user profile page.</returns>
        public async Task<ActionResult> GetAccountSettingsPartial()
        {
            var username = User.Identity.Name;

            var userByUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.UserName", username }
                }).ToListAsync();

            return View("AccountSettingsEdit", userByUsername[0]);
        }

        /// <summary>
        /// Get method for the Add skills partial
        /// </summary>
        /// <returns>returns the add skills partial when the user clicks edit/add skills button.</returns>
        public async Task<ActionResult> GetAddSkillsPartial()
        {
            if (User != null)
            {
                var username = User.Identity.Name;

                var userByUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument
                {
                    {"Account.UserName", username}
                }).ToListAsync();

                var userTradingDetails = await DatabaseContext.LetsTradingDetails.Find(new BsonDocument
                {
                    {"_id", userByUsername[0].Id}
                }).ToListAsync();

                return View("AddSkills", userTradingDetails[0]);
            }
            return null;
        }

        /// <summary>
        /// Post method for the skills edit partial
        /// </summary>
        /// <param name="letsTradingDetails">stores the skills entered by the user.</param>
        /// <returns>reloads the user profile page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SkillsSettingsEdit(LetsTradingDetails letsTradingDetails)
        {
            await AddSkill(letsTradingDetails.Skill);
            return RedirectToAction("UserProfile", "Account");
        }

        /// <summary>
        /// Adds the skill to the user profile
        /// </summary>
        /// <param name="skill">stores the skill entered by the user</param>
        /// <returns>adds the skill and returns the skills partial</returns>
        public async Task<ActionResult> AddSkill(string skill)
        {
            var username = User.Identity.Name;

            var userByUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.UserName", username }
                }).ToListAsync();

            var userTradingDetails = await DatabaseContext.LetsTradingDetails.Find(new BsonDocument {
                    { "_id", userByUsername[0].Id }
                }).ToListAsync();

            if (userTradingDetails[0].Skills == null || userTradingDetails[0].Skills.Count == 0)
            {
                userTradingDetails[0].Skills = new List<string>();
            }

            if (!string.IsNullOrEmpty(skill) && !userTradingDetails[0].Skills.Contains(skill))
            {
                userTradingDetails[0].Skills.Add(skill);
                await DatabaseContext.LetsTradingDetails.ReplaceOneAsync(r => r.Id == userTradingDetails[0].Id, userTradingDetails[0]);
            }

            return View("AddedUserSkills", userTradingDetails[0]);
        }

        /// <summary>
        /// Removes the user skills from the user profile
        /// </summary>
        /// <param name="skill">stores the skill that needs to be removed from the user profile</param>
        /// <returns>removes the user skill and returns the skills partial</returns>
        public async Task<ActionResult> RemoveSkill(string skill)
        {
            if (User != null)
            {
                var username = User.Identity.Name;

                var userByUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.UserName", username }
                }).ToListAsync();

                var userTradingDetails = await DatabaseContext.LetsTradingDetails.Find(new BsonDocument {
                    { "_id", userByUsername[0].Id }
                }).ToListAsync();

                userTradingDetails[0].Skills.Remove(skill);
                await DatabaseContext.LetsTradingDetails.ReplaceOneAsync(r => r.Id == userTradingDetails[0].Id, userTradingDetails[0]);
            }
            return null;
        }

        /// <summary>
        /// Allows the user to change their account passwords
        /// </summary>
        /// <param name="registeredUser">stores the users old password, new password and confirm password</param>
        /// <returns>verifies and changes the password and reloads the user profile page with an notification message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(RegisterUserViewModel registeredUser)
        {
            if (User != null)
            {
                var username = User.Identity.Name;
                var userByUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument
                {
                    {"Account.UserName", username}
                }).ToListAsync();
                var passwordEncryption = new PasswordHashAndSalt();
                var oldPassword = passwordEncryption.getHashedPassword(registeredUser.Account.OldPassword);
                var newPassword = passwordEncryption.getHashedPassword(registeredUser.Account.NewPassword);
                var confirmNewPassword = passwordEncryption.getHashedPassword(registeredUser.Account.ConfirmNewPassword);

                if (userByUsername != null && userByUsername.Count > 0 && newPassword.Equals(confirmNewPassword))
                {
                    if (userByUsername[0].Account.Password.Equals(oldPassword) ||
                        (!string.IsNullOrEmpty(userByUsername[0].Account.TempPassword) &&
                         userByUsername[0].Account.TempPassword.Equals(oldPassword)))
                    {
                        userByUsername[0].Account.Password = newPassword;
                        userByUsername[0].Account.TempPassword = null;
                        await DatabaseContext.RegisteredUsers.ReplaceOneAsync(r => r.Account.UserName == userByUsername[0].Account.UserName, userByUsername[0]);
                        TempData.Add("PasswordChanged", "Your Password was changed successfully.");
                    }
                    else
                    {
                        TempData.Add("PasswordNotChanged", "There was an error in changing you password. Please try again.");
                    }
                }
            }
            else
            {
                TempData.Add("PasswordNotChanged", "There was an error in changing you password. Please try again.");
            }
            return RedirectToAction("UserProfile", "Account");
        }
    }
}