using System.Threading.Tasks;
using LETS.Helpers;
using LETS.Models;
using System.Web.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

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
            userTimeLinePosts.UserTimelinePostsList = userTimeLinePosts.UserTimelinePostsList.Take(12).ToList();
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

        [HttpPost]
        public async Task<ActionResult> ExpandPost(string username, int postId)
        {
            var user = username;

            var userByUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument
                {
                    {"Account.UserName", user}
                }).ToListAsync();

            var userTradingDetails = await DatabaseContext.LetsTradingDetails.Find(new BsonDocument
                {
                    {"_id", userByUsername[0].Id}
                }).ToListAsync();
            
            var userPost = new UsersTimeLinePost();
            var post = userTradingDetails[0].Requests.ElementAt(postId);
            userPost.FirstName = userByUsername[0].About.FirstName;
            userPost.LastName = userByUsername[0].About.LastName;
            userPost.UserName = userByUsername[0].Account.UserName;
            userPost.Request = new RequestPost
            {
                Date = post.Date,
                Budget = post.Budget,
                Description = post.Description,
                Tags = post.Tags,
                Title = post.Title,
                Id = post.Id,
                Bids = post.Bids
            };
            return View("ExpandedRequest", userPost);
        }

        [HttpPost]
        public async Task<ActionResult> PostUserBid(string username, int postId, float bid)
        {
            var user = username;

            var userByUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument
                {
                    {"Account.UserName", user}
                }).ToListAsync();

            var userTradingDetails = await DatabaseContext.LetsTradingDetails.Find(new BsonDocument
                {
                    {"_id", userByUsername[0].Id}
                }).ToListAsync();

            var post = userTradingDetails[0].Requests.ElementAt(postId);

            var userBid = new Bid
            {
                Username = User.Identity.Name,
                Amount = bid
            };

            if (post.Bids == null)
            {
                post.Bids = new List<Bid>();
            }

            if (post.Bids.Find(item => item.Username.Equals(User.Identity.Name)) != null)
            {
                var existingBid = post.Bids.Find(item => item.Username.Equals(User.Identity.Name));
                post.Bids.Remove(existingBid);
            }

            post.Bids.Add(userBid);

            await DatabaseContext.LetsTradingDetails.ReplaceOneAsync(r => r.Id == userTradingDetails[0].Id, userTradingDetails[0]);

            return View("UsersBidChip", userBid);
        }
    }
}