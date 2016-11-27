using System;
using System.Web;
using System.Web.Mvc;
using LETS.Controllers;
using NUnit.Framework;
using LETS.Models;
using MongoDB.Bson;
using LETS.Helpers;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using System.Threading.Tasks;

namespace LETS.Tests.Controllers
{
    public class RegisterUserTests : AssertionHelper
    {
        [Test]
        public void ToDocument_UserWithAnId_IdRepresentedAsAnObjectId()
        {
            var user = new RegisterUserViewModel {Id = Guid.NewGuid().ToString()};

            var document = user.ToBsonDocument();
            Expect(document["_id"].BsonType, Is.EqualTo(BsonType.String));
        }

       [Test]
        public void TestLogin1()
        {
            var controller = new AccountController();
            var loginUser = new LoginViewModel
            {
                UserName = "admin",
                Password = "admin"
            };
            var result = controller.Login(loginUser);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void TestLogin2()
        {
            var controller = new AccountController();
            var loginUser = new LoginViewModel
            {
                UserName = "KiranRambha",
                Password = "admin"
            };
            var result = controller.Login(loginUser);
            Assert.IsInstanceOf(typeof(ViewResult), result);
        }

        [Test]
        public void TestCheckUserName()
        {
            var controller = new AccountController();
            var result = controller.CheckUserName("KiranRambha");
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
    }
}
