using System.Threading.Tasks;
using LETS.Helpers;
using LETS.Models;
using System.Web.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
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
        public async Task<ActionResult> TimeLine()
        {
            var username = User.Identity.Name;
            var userByUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.UserName", username }
                }).ToListAsync();

            var userTradingDetails = await DatabaseContext.LetsTradingDetails.Find(new BsonDocument {
                    { "_id", userByUsername[0].Id }
                }).ToListAsync();

            var userTimeLinePosts = new UserTimeLinePostsList();
            var usersPersonalDetails = await DatabaseContext.RegisteredUsers.Find(_ => true).ToListAsync();
            var usersTradingDetails = await DatabaseContext.LetsTradingDetails.Find(_ => true).ToListAsync();
            var userid = usersPersonalDetails.Find(user => user.Account.UserName.Equals(username)).Id;
            var currentUserPersonalDetails = usersPersonalDetails.Find(user => user.Id.Equals(userid));
            var currentUserTradingDetails = usersTradingDetails.Find(user => user.Id.Equals(userid));
            usersPersonalDetails.Remove(currentUserPersonalDetails);
            usersTradingDetails.Remove(currentUserTradingDetails);
            userTimeLinePosts.UserTimelinePostsList = GetUserTimelinePosts(usersPersonalDetails, usersTradingDetails);
            userTimeLinePosts.UserTimelinePostsList.Reverse();
            userTimeLinePosts.UserTimelineRecommendedPostsList = GetUserTimelineRecommendedPosts(userTradingDetails[0], usersPersonalDetails, usersTradingDetails);
            return View("TimeLine", userTimeLinePosts);
        }

        private static List<UsersTimeLinePost> GetUserTimelinePosts(List<RegisterUserViewModel> usersPersonalDetails, List<LetsTradingDetails> usersTradingDetails)
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
                        if (string.IsNullOrEmpty(request.IsAssignedTo) && !request.HasDeleted)
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
            }

            timelinePostsList = timelinePostsList.OrderBy(post => post.Request.Date).ToList();
            return timelinePostsList;
        }

        private static List<UsersTimeLinePost> GetUserTimelineRecommendedPosts(LetsTradingDetails currentuser, List<RegisterUserViewModel> usersPersonalDetails, List<LetsTradingDetails> usersTradingDetails)
        {
            var currentUser = currentuser;

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
                        if (string.IsNullOrEmpty(request.IsAssignedTo) && !request.HasDeleted)
                        {
                            foreach (var tag in request.Tags)
                            {
                                if (currentUser.Skills.Contains(tag))
                                {
                                    var timelinePost = new UsersTimeLinePost
                                    {
                                        ImageId = user.UserPersonalDetails.Account.ImageId,
                                        UserName = user.UserPersonalDetails.Account.UserName,
                                        FirstName = user.UserPersonalDetails.About.FirstName,
                                        LastName = user.UserPersonalDetails.About.LastName,
                                        Request = request
                                    };

                                    if (!timelinePostsList.Contains(timelinePost))
                                    {
                                        timelinePostsList.Add(timelinePost);
                                    }
                                }
                            }
                        }
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

            var loggedUser = User.Identity.Name;

            var loggedUserByUserName = await DatabaseContext.RegisteredUsers.Find(new BsonDocument
                {
                    {"Account.UserName", loggedUser}
                }).ToListAsync();

            var loggedTradingDetails = await DatabaseContext.LetsTradingDetails.Find(new BsonDocument
                {
                    {"_id", loggedUserByUserName[0].Id}
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
                Bids = post.Bids,
                MyCredits = loggedTradingDetails[0].Credit
            };
            return View("ExpandedRequest", userPost);
        }

        [HttpPost]
        public async Task<ActionResult> PostUserBid(string username, int postId, int bid)
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

            var loggedUser = User.Identity.Name;

            var loggedUserByUserName = await DatabaseContext.RegisteredUsers.Find(new BsonDocument
                {
                    {"Account.UserName", loggedUser}
                }).ToListAsync();

            var loggedTradingDetails = await DatabaseContext.LetsTradingDetails.Find(new BsonDocument
                {
                    {"_id", loggedUserByUserName[0].Id}
                }).ToListAsync();

            if (loggedTradingDetails[0].Credit <= 200)
            {
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

                await
                    DatabaseContext.LetsTradingDetails.ReplaceOneAsync(r => r.Id == userTradingDetails[0].Id,
                        userTradingDetails[0]);

                return View("UsersBidChip", userBid);
            }
            else
            {
                return null;
            }
        }

        public ActionResult Chat()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> TeamManagement()
        {
            var userName = User.Identity.Name;
            var listOfTeams = new List<TeamManagement>();

            var teamByMembership = await DatabaseContext.LetsTeamsDatabase.Find(new BsonDocument {
                    { "TeamMembers", new BsonDocument { { "$elemMatch", new BsonDocument { { "UserName", userName } } } } }
                }).ToListAsync();

            var allTeams = new AllTeams
            {
                AllTeamsList = teamByMembership,
                Team = new TeamManagement()
            };
            return View(allTeams);
        }

        [HttpPost]
        public ActionResult TeamManagement(string username)
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateTeamRequest(string teamName, string teamMembers)
        {
            var teamMembersArray = teamMembers.Split(',');
            var teamMembersList = new List<string>(teamMembersArray.Length);
            teamMembersList.AddRange(teamMembersArray);

            var team = new TeamManagement
            {
                Id = Guid.NewGuid().ToString(),
                TeamName = teamName,
                TeamMembers = new List<Member>(),
                Admin = User.Identity.Name
            };

            var adminUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.UserName", User.Identity.Name }
                }).ToListAsync();

            var member = new Member
            {
                UserName = adminUsername[0].Account.UserName,
                FirstName = adminUsername[0].About.FirstName,
                LastName = adminUsername[0].About.LastName
            };

            team.TeamMembers.Add(member);

            foreach (var user in teamMembersList)
            {
                var userByUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.UserName", user }
                }).ToListAsync();

                member = new Member
                {
                    UserName = userByUsername[0].Account.UserName,
                    FirstName = userByUsername[0].About.FirstName,
                    LastName = userByUsername[0].About.LastName
                };
                team.TeamMembers.Add(member);
            }

            DatabaseContext.LetsTeamsDatabase.InsertOne(team);

            return View("YourTeamTemplate", team);
        }

        [HttpGet]
        public async Task<JsonResult> GetUserNames(string username)
        {
            var filter = new BsonDocument { { "Account.UserName", new BsonDocument { { "$regex", ".*" + username + ".*" }, { "$options", "i" } } } };

            var userByUsername = await DatabaseContext.RegisteredUsers.Find(filter).ToListAsync();

            var userNameList = userByUsername.Select(user => user.Account.UserName).ToList();

            return Json(userNameList.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AddUser(string username)
        {
            var userByUsername = await DatabaseContext.RegisteredUsers.Find(new BsonDocument {
                    { "Account.UserName", username }
                }).ToListAsync();

            if (userByUsername != null && userByUsername.Count > 0 && !userByUsername[0].Account.UserName.Equals(User.Identity.Name))
            {
                return View("AddedUserName", userByUsername[0]);
            }
            else
            {
                return null;
            }
        }

        public ActionResult SendMessage()
        {
            return null;
        }

        public async Task<ActionResult> SearchPosts(string searchInput)
        {
            var isPresent = false;
            var username = User.Identity.Name;
            var userTimeLinePosts = new UserTimeLinePostsList();
            var usersPersonalDetails = await DatabaseContext.RegisteredUsers.Find(_ => true).ToListAsync();
            var usersTradingDetails = await DatabaseContext.LetsTradingDetails.Find(_ => true).ToListAsync();
            var userid = usersPersonalDetails.Find(user => user.Account.UserName.Equals(username)).Id;
            var currentUserPersonalDetails = usersPersonalDetails.Find(user => user.Id.Equals(userid));
            var currentUserTradingDetails = usersTradingDetails.Find(user => user.Id.Equals(userid));
            usersPersonalDetails.Remove(currentUserPersonalDetails);
            usersTradingDetails.Remove(currentUserTradingDetails);
            userTimeLinePosts.UserTimelinePostsList = new List<UsersTimeLinePost>();
            var tempList = GetUserTimelinePosts(usersPersonalDetails, usersTradingDetails);
            foreach (var post in tempList)
            {
                foreach (var tag in post.Request.Tags)
                {
                    if (string.IsNullOrEmpty(searchInput) || tag.ToLower().Equals(searchInput.ToLower()))
                    {
                        isPresent = true;
                    }
                }

                if (isPresent)
                {
                    userTimeLinePosts.UserTimelinePostsList.Add(post);
                    isPresent = false;
                }
            }
            userTimeLinePosts.UserTimelinePostsList.Reverse();
            return View("TimeLineFiltered", userTimeLinePosts);
        }

        public async Task<bool> DeleteTeam(string teamId)
        {
            var userName = User.Identity.Name;

            var teamByMembership = await DatabaseContext.LetsTeamsDatabase.Find(new BsonDocument {
                    { "TeamMembers", new BsonDocument { { "$elemMatch", new BsonDocument { { "UserName", userName } } } } }
                }).ToListAsync();

            foreach (var team in teamByMembership)
            {
                if (team.Id.Equals(teamId))
                {
                    team.IsDeleted = true;
                    await DatabaseContext.LetsTeamsDatabase.ReplaceOneAsync(r => r.Id == team.Id, team);
                }
            }

            return true;
        }

        public async Task<ActionResult> GetLatestMessages(string teamId)
        {
            var team = teamId.Substring(0, teamId.Length - "-messagebox".Length);
            var teamByMembership = await DatabaseContext.LetsTeamsDatabase.Find(new BsonDocument
                {
                    {"_id", team}
                }).ToListAsync();
            return View("GetLatestMessages", teamByMembership[0]);
        }
    }
}