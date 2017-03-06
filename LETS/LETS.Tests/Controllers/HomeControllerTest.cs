using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LETS;
using Assert = NUnit.Framework.Assert;
using LETS.Controllers;
using LETS.Models;
using Moq;
using NUnit.Framework;

namespace LETS.Tests.Controllers
{
    public class HomeControllerTest : AssertionHelper
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
        public void TestIndex()
        {
            var controller = new HomeController();

            var result = controller.Index() as ViewResult;

            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void TestComponentsGuide()
        {
            var controller = new HomeController();

            var result = controller.ComponentsGuide() as ViewResult;

            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void TestChat()
        {
            var controller = new HomeController();

            var result = controller.Chat() as ViewResult;

            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void TestTeamManagement()
        {
            var controller = new HomeController();

            var result = controller.TeamManagement("KiranRambha") as ViewResult;

            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void TestSendMessage()
        {
            var controller = new HomeController();

            var result = controller.SendMessage();

            Assert.IsNull(result);
        }

        [Test]
        public void TestComponentsGuide1()
        {
            var controller = new HomeController();

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

            var result = controller.ComponentsGuide(registerUser);

            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
        }

        [Test]
        public void TestComponentsGuide2()
        {
            var controller = new HomeController();

            var result = controller.ComponentsGuide(null);

            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public async Task TestAddUser()
        {
            var controller = new HomeController { ControllerContext = MockControllerContext() };
            var result = await controller.AddUser("KiranRambha");
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public async Task TestAddUser1()
        {
            var controller = new HomeController { ControllerContext = MockControllerContext() };
            var result = await controller.AddUser("beastmaster");
            Assert.IsNull(result);
        }

        [Test]
        public async Task TestTeamManagement1()
        {
            var controller = new HomeController { ControllerContext = MockControllerContext() };
            var result = await controller.TeamManagement();
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }


        [Test]
        public async Task TestExpandPost()
        {
            var controller = new HomeController { ControllerContext = MockControllerContext() };
            var result = await controller.ExpandPost("beastmaster", 0);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public async Task TestCreateTeamRequest()
        {
            var controller = new HomeController { ControllerContext = MockControllerContext() };
            var result = await controller.CreateTeamRequest("Temp Team", "KiranRambha,test,AmanRawal");
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public async Task TestGetUserNames()
        {
            var controller = new HomeController { ControllerContext = MockControllerContext() };
            var result = await controller.GetUserNames("Kiran");
            Assert.IsInstanceOf(typeof(JsonResult), result);
        }

        [Test]
        public async Task TestSearchPosts1()
        {
            var controller = new HomeController { ControllerContext = MockControllerContext() };
            var result = await controller.SearchPosts("test1");
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public async Task TestSearchPosts2()
        {
            var controller = new HomeController { ControllerContext = MockControllerContext() };
            var result = await controller.SearchPosts("");
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public async Task TestPostUserBid()
        {
            var controller = new HomeController { ControllerContext = MockControllerContext() };
            var result = await controller.PostUserBid("beastmaster", 0, 1);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public async Task TestTimeline()
        {
            var controller = new HomeController { ControllerContext = MockControllerContext() };
            var result = await controller.TimeLine();
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }
    }
}
