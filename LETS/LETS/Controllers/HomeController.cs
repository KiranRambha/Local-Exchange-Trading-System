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

        /// <summary>
        /// Get Method of the Index
        /// </summary>
        /// <returns>returns the index view that is the home page of the website.</returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// When the user clicks the timeline button on the website, this method gets called
        /// </summary>
        /// <returns>returns the timeline page</returns>
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


        /// <summary>
        /// This method is used to create the list of posts on the user timeline.
        /// </summary>
        /// <param name="usersPersonalDetails">Users Personal Details</param>
        /// <param name="usersTradingDetails">Users Trading Details</param>
        /// <returns>Returns the list of posts that will be displayed on the timeline page.</returns>
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


        /// <summary>
        /// This method is used to create the list of recommended posts on the user timeline.
        /// </summary>
        /// <param name="currentuser">Logged in user details</param>
        /// <param name="usersPersonalDetails">Users personal details</param>
        /// <param name="usersTradingDetails">Users trading details</param>
        /// <returns>Creates and returns a recommended user list</returns>
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

        /// <summary>
        /// This method is called when the user clicks on the expand post button on the timeline
        /// </summary>
        /// <param name="username">username of the post owner</param>
        /// <param name="postId">Id of the post clicked</param>
        /// <returns>Returns the details of the post that was clicked.</returns>
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

        /// <summary>
        /// This method is used to make bids on the timeline
        /// </summary>
        /// <param name="username">username of the owner of the post</param>
        /// <param name="postId">postid of the owner where the bid was placed</param>
        /// <param name="bid">the bid amount that was placed on the post</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get method for the team management page.
        /// </summary>
        /// <returns>returns the team management when the user clicks the team management button</returns>
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

        /// <summary>
        /// This method is used to create a team on the team management page.
        /// </summary>
        /// <param name="teamName">The name of the team</param>
        /// <param name="teamMembers">The list of members in the team</param>
        /// <returns></returns>
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

        /// <summary>
        /// This method is used to get the usernames of the database.
        /// </summary>
        /// <param name="username">a string to match similar usernames in the database.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetUserNames(string username)
        {
            var filter = new BsonDocument { { "Account.UserName", new BsonDocument { { "$regex", ".*" + username + ".*" }, { "$options", "i" } } } };

            var userByUsername = await DatabaseContext.RegisteredUsers.Find(filter).ToListAsync();

            var userNameList = userByUsername.Select(user => user.Account.UserName).ToList();

            return Json(userNameList.ToArray(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This method is used to add the username to a team.
        /// </summary>
        /// <param name="username">the username that needs to be added to the team</param>
        /// <returns>returns the chip with the username that is going to be added to the team.</returns>
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

        /// <summary>
        /// This method is used to search for posts on the timeline page of the web application.
        /// </summary>
        /// <param name="searchInput">The string that is being searched</param>
        /// <returns>returns the list of posts that match the search string.</returns>
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


        /// <summary>
        /// This method is used to to delete a team.
        /// </summary>
        /// <param name="teamId">The id of the team that needs to be deleted.</param>
        /// <returns>returns true when the team has been deleted.</returns>
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

        /// <summary>
        /// This method is used to get all the latest messages in the team chat.
        /// </summary>
        /// <param name="teamId">The id of the team for which the messages need to be acquired.</param>
        /// <returns>returns the lastest list of messages of the selected team.</returns>
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