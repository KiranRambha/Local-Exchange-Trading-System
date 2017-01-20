using System.Threading.Tasks;
using LETS.Helpers;
using LETS.Models;
using System.Web.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public async Task<ActionResult> Home()
        {
            var username = User.Identity.Name;
            var userTimeLinePosts = new UserTimeLinePostsList();
            var usersPersonalDetails = await DatabaseContext.RegisteredUsers.Find(_ => true).ToListAsync();
            var usersTradingDetails = await DatabaseContext.LetsTradingDetails.Find(_ => true).ToListAsync();
            var userid = usersPersonalDetails.Find(user => user.Account.UserName.Equals(username)).Id;
            var currentUserPersonalDetails = usersPersonalDetails.Find(user => user.Id.Equals(userid));
            var currentUserTradingDetails = usersTradingDetails.Find(user => user.Id.Equals(userid));
            usersPersonalDetails.Remove(currentUserPersonalDetails);
            usersTradingDetails.Remove(currentUserTradingDetails);
            userTimeLinePosts.UserTimelinePostsList = GetUserTimeListPosts(usersPersonalDetails, usersTradingDetails);
            userTimeLinePosts.UserTimelinePostsList.Reverse();
            return View(userTimeLinePosts);
        }

        private static List<UsersTimeLinePost> GetUserTimeListPosts(List<RegisterUserViewModel> usersPersonalDetails, List<LetsTradingDetails> usersTradingDetails)
        {
            var userList = new List<LetsUser>();
            var timelinePostsList = new List<UsersTimeLinePost>();

            //Building a list that contains all the users personal details and trading details
            foreach (var userPersonalDetails in usersPersonalDetails)
            {
                var userTradingDetails = usersTradingDetails.Find(item => item.Id.Equals(userPersonalDetails.Id));
                var letsUser = new LetsUser
                {
                    UserPersonalDetails = userPersonalDetails,
                    UserTradingDetails = userTradingDetails
                };
                userList.Add(letsUser);
            }

            foreach (var user in userList)
            {
                if (user.UserTradingDetails.Requests != null)
                {
                    foreach (var request in user.UserTradingDetails.Requests)
                    {
                        var timelinePost = new UsersTimeLinePost
                        {
                            ImageId = user.UserPersonalDetails.Account.ImageId,
                            UserName = user.UserPersonalDetails.Account.UserName,
                            FirstName = user.UserPersonalDetails.About.FirstName,
                            LastName = user.UserPersonalDetails.About.LastName,
                            Request = request
                        };
                        timelinePostsList.Add(timelinePost);
                    }
                }
            }

            timelinePostsList = timelinePostsList.OrderBy(post => post.Request.Date).ToList();
            return timelinePostsList;
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