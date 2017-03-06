using System;
using System.Collections.Generic;
using System.Web.Mvc;
using LETS.Controllers;
using NUnit.Framework;
using LETS.Models;
using MongoDB.Bson;
using Assert = NUnit.Framework.Assert;
using System.Configuration;
using System.IO;
using System.Security.Principal;
using System.Net.Http;
using System.Net.Mail;
using System.Runtime.Versioning;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using Microsoft.Owin;
using Moq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.UI.WebControls;
using MongoDB.Driver.Core.Operations;

namespace LETS.Tests.Controllers
{
    public class AccountControllerTests : AssertionHelper
    {
        public AccountController InitOwinContext(AccountController controller)
        {
            var url = new Uri("https://localhost/Account/Loginn");
            var routeData = new RouteData();

            var httpRequest = new HttpRequest("", url.AbsoluteUri, "");
            var httpResponse = new HttpResponse(null);
            var httpContext = new HttpContext(httpRequest, httpResponse);
            var owinEnvironment = new Dictionary<string, object>()
            {
                {"owin.RequestBody", null}
            };
            httpContext.Items.Add("owin.Environment", owinEnvironment);
            var contextWrapper = new HttpContextWrapper(httpContext);

            var controllerContext = new ControllerContext(contextWrapper, routeData, controller);
            controller.ControllerContext = controllerContext;
            controller.Url = new UrlHelper(new RequestContext(contextWrapper, routeData));
            return controller;
        }

        public ControllerContext MockControllerContext()
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("user")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("beastmaster");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            return controllerContext.Object;
        }

        [Test]
        public void ToDocument_UserWithAnId_IdRepresentedAsAnObjectId()
        {
            var user = new RegisterUserViewModel {Id = Guid.NewGuid().ToString()};

            var document = user.ToBsonDocument();
            Expect(document["_id"].BsonType, Is.EqualTo(BsonType.String));
        }

       [Test]
        public async Task TestLogin1()
        {
            var controller = new AccountController();
            var loginUser = new LoginViewModel
            {
                UserName = "admin",
                Password = "admin"
            };
            var result = await controller.Login(loginUser);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public async Task TestLogin2()
        {
            var controller = new AccountController();

            var loginUser = new LoginViewModel
            {
                UserName = "beastmaster",
                Password = "admin"
            };

            var result = await controller.Login(loginUser);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public async Task TestLogin3()
        {
            var controller = new AccountController();

            var loginUser = new LoginViewModel
            {
                UserName = "beastmaster",
                Password = "4wordsallUpper"
            };

            controller = InitOwinContext(controller);
            var result = await controller.Login(loginUser);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public void TestLogin4()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(true);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var controller = new AccountController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            var result = controller.Login();
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public void TestLogin5()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(false);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var controller = new AccountController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            var result = controller.Login();
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void TestCheckUserName()
        {
            var controller = new AccountController();
            var result = controller.CheckUserName("beastmaster");
            Assert.IsFalse(result);
        }

        [Test]
        public void TestRegisterUser()
        {
            var controller = new AccountController();
            var registerUser = new RegisterUserViewModel
            {
                Account = new Account(),
                About = new About()
            };
            registerUser.Account.UserName = "admin";
            registerUser.Account.Password = "admin";
            registerUser.Account.ConfirmPassword = "admin";
            registerUser.Account.Email = "admin@admin.com";
            registerUser.About.FirstName = "admin";
            registerUser.About.LastName = "admin";
            registerUser.About.Gender = "admin";
            registerUser.About.Title = "admin";
            var result = controller.Register(registerUser);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void TestRegisteredUsers()
        {
            var controller = new AccountController();
            var result = controller.RegisteredUsers();
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public async Task TestGetAddSkillsPartial1()
        {
            var controller = new AccountController();
            var result = await controller.GetAddSkillsPartial();
            Assert.AreEqual(null, result);
        }

        [Test]
        public async Task TestGetAddSkillsPartial2()
        {
            var controller = new AccountController {ControllerContext = MockControllerContext()};
            var result = await controller.GetAddSkillsPartial();
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void TestSendEmail()
        {
            var controller = new AccountController();
            using (var mail = new MailMessage())
            {
                mail.To.Add("kiran.rambha.1995@gmail.com");
                mail.Subject = "Unit Testing";
                mail.Body = "<p>Unit Testing</p>";
                ConfigurationManager.AppSettings["RHLETS.Email"] = "rhulletsteam@gmail.com";
                ConfigurationManager.AppSettings["RHLETS.Password"] = "zyvb492@rhul@egham";
                controller.SendEmail(mail);
                Assert.True(true);
            }
        }

        [Test]
        public async Task TestAddSkill()
        {
            var controller = new AccountController {ControllerContext = MockControllerContext()};
            var result = await controller.AddSkill("Test");
            var dump = await controller.RemoveSkill("Test");
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public async Task TestRemoveSkill()
        {
            var controller = new AccountController {ControllerContext = MockControllerContext()};
            var result = await controller.RemoveSkill("Test");
            Assert.AreEqual(null, result);
        }

        [Test]
        public void TestForgotUserName1()
        {
            var controller = new AccountController();
            controller = InitOwinContext(controller);
            var result = controller.ForgotUsername();
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void TestForgotPassword1()
        {
            var controller = new AccountController();
            controller = InitOwinContext(controller);
            var result = controller.ForgotPassword();
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public async Task TestForgotUserName2()
        {
            var controller = new AccountController();
            var forgotUserName = new ForgotUsernameViewModel { Email = "k@r.com" };
            controller = InitOwinContext(controller);
            var result = await controller.ForgotUsername(forgotUserName);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public async Task TestForgotPassword2()
        {
            var controller = new AccountController();
            var forgotPassword = new ForgotPasswordViewModel
            {
                Email = "k@r.com",
                UserName = "beastmaster"
            };
            controller = InitOwinContext(controller);
            var result = await controller.ForgotPassword(forgotPassword);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void TestForgotUserName3()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(true);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var controller = new AccountController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            var result = controller.ForgotUsername();
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public void TestForgotPassword3()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(true);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var controller = new AccountController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            var result = controller.ForgotPassword();
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public async Task TestChangePassword1()
        {
            var account = new Account
            {
                OldPassword = "4wordsallUpper",
                NewPassword = "4wordsallUpper",
                ConfirmNewPassword = "4wordsallUpper"
            };

            var user = new RegisterUserViewModel
            {
                Account = account
            };
            var controller = new AccountController { ControllerContext = MockControllerContext() };
            var result = await controller.ChangePassword(user);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public async Task TestChangePassword2()
        {
            var account = new Account
            {
                OldPassword = "4wordsallUpper",
                NewPassword = "4wordsallUpper",
                ConfirmNewPassword = "4wordsallUpper"
            };

            var user = new RegisterUserViewModel
            {
                Account = account
            };
            var controller = new AccountController();
            var result = await controller.ChangePassword(user);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public async Task TestChangePassword3()
        {
            var account = new Account
            {
                OldPassword = "4wordsallUpper1",
                NewPassword = "4wordsallUpper",
                ConfirmNewPassword = "4wordsallUpper"
            };

            var user = new RegisterUserViewModel
            {
                Account = account
            };
            var controller = new AccountController { ControllerContext = MockControllerContext() };
            var result = await controller.ChangePassword(user);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public async Task TestAccountSettingsEdit()
        {
            var controller = new AccountController { ControllerContext = MockControllerContext() };
            var registerUser = new RegisterUserViewModel
            {
                Account = new Account(),
                About = new About()
            };
            registerUser.Account.Email = "kiran.rambha@outlook.com";
            registerUser.About.FirstName = "test";
            registerUser.About.LastName = "test";
            registerUser.About.Gender = "other";
            registerUser.About.Title = "mr";
            var result = await controller.AccountSettingsEdit(registerUser);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public async Task TestUserProfile()
        {
            var controller = new AccountController { ControllerContext = MockControllerContext() };
            var result = await controller.UserProfile();
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void TestRegister1()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(true);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var controller = new AccountController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            var result = controller.Register();
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public void TestRegister2()
        {
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.IsAuthenticated).Returns(false);

            var context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);

            var controller = new AccountController();
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            var result = controller.Register();
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void TestLogoff()
        {
            var controller = new AccountController();
            controller = InitOwinContext(controller);
            var result = controller.Logoff();
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public async Task TestGetAccountSettingsPartial()
        {
            var controller = new AccountController { ControllerContext = MockControllerContext() };
            var result = await controller.GetAccountSettingsPartial();
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public async Task TestSkillsSettingsEdit()
        {
            var letsTradingDetails = new LetsTradingDetails {Skill = "Test"};
            var controller = new AccountController { ControllerContext = MockControllerContext() };
            var result = await controller.SkillsSettingsEdit(letsTradingDetails);
            var dump = await controller.RemoveSkill("Test");
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public void TestCreatePassword()
        {
            var controller = new AccountController();
            var result = controller.CreatePassword();
            Assert.IsInstanceOf(typeof(string), result);
        }

        [Test]
        public void TestExpandPost()
        {
            var controller = new AccountController();
            var result = controller.ExpandPost("beastmaster", 0);
            Assert.IsInstanceOf(typeof(Task<ActionResult>), result);
        }

        [Test]
        public void TestGetUserSkills()
        {
            var controller = new AccountController();
            var result = controller.GetUserSkills("ASP.NET");
            Assert.IsInstanceOf(typeof(Task<JsonResult>), result);
        }

        [Test]
        public void TestPostRequest()
        {
            var controller = new AccountController { ControllerContext = MockControllerContext() };
            var result = controller.PostRequest("test", "test", 1, "test1,test2");
            Assert.IsInstanceOf(typeof(Task<ActionResult>), result);
        }

        [Test]
        public void TestGetProfilePicture()
        {
            var controller = new AccountController { ControllerContext = MockControllerContext() };
            var result = controller.GetProfilePicture();
            Assert.IsInstanceOf(typeof(Task<ActionResult>), result);
        }

        [Test]
        public void TestAcceptUserBid()
        {
            var controller = new AccountController { ControllerContext = MockControllerContext() };
            var result = controller.AcceptUserBid("test", 0);
            Assert.IsInstanceOf(typeof(Task<bool>), result);
        }

        [Test]
        public void TestChangeProfilePicture1()
        {
            var controller = new AccountController { ControllerContext = MockControllerContext() };
            var result = controller.ChangeProfilePicture();
            Assert.IsInstanceOf(typeof(Task<ActionResult>), result);
        }

        [Test]
        public void TestDeleteImage()
        {
            var controller = new AccountController { ControllerContext = MockControllerContext() };
            var result = controller.DeleteImage("1234");
            Assert.IsInstanceOf(typeof(Task), result);
        }

        [Test]
        public void TestChangeProfilePicture2()
        {
            var controller = new AccountController { ControllerContext = MockControllerContext() };
            Mock<ControllerContext> cc = new Mock<ControllerContext>();
            UTF8Encoding enc = new UTF8Encoding();

            Mock<HttpPostedFileBase> file1 = new Mock<HttpPostedFileBase>();
            file1.Expect(d => d.FileName).Returns("~/Content/images/DefaultProfile.jpg");
            file1.Expect(d => d.ContentType).Returns("image/jpeg");
            file1.Expect(d => d.InputStream).Returns(new MemoryStream(enc.GetBytes("~/Content/images/DefaultProfile.jpg")));

            cc.Expect(d => d.HttpContext.Request.Files.Count).Returns(2);
            cc.Expect(d => d.HttpContext.Request.Files[0]).Returns(file1.Object);

            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("user")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("beastmaster");
            cc.SetupGet(x => x.HttpContext.User).Returns(principal.Object);

            controller.ControllerContext = cc.Object;

            var result = controller.ChangeProfilePicture();
            Assert.IsInstanceOf(typeof(Task<ActionResult>), result);
        }

        [Test]
        public void TestArchiveJob()
        {
            var controller = new AccountController { ControllerContext = MockControllerContext() };
            var result = controller.ArchiveJob(0);
            Assert.IsInstanceOf(typeof(Task), result);
        }

        [Test]
        public void TestDeleteRequest()
        {
            var controller = new AccountController { ControllerContext = MockControllerContext() };
            var result = controller.DeleteRequest(0);
            Assert.IsInstanceOf(typeof(Task<Boolean>), result);
        }

        [Test]
        public void TestGetUserJobs()
        {
            var controller = new AccountController { ControllerContext = MockControllerContext() };
            var result = controller.GetUserJobs();
            Assert.IsInstanceOf(typeof(Task<ActionResult>), result);
        }
    }
}
