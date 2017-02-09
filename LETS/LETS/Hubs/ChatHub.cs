using System.Collections.Generic;
using LETS.Helpers;
using LETS.Models;
using Microsoft.AspNet.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LETS.Hubs
{
    public class ChatHub : Hub
    {
        public LETSContext DatabaseContext = new LETSContext();

        public void Send(string name, string message, string teamID)
        {
            Clients.All.addNewMessageToPage(name, message);

            var teamById = DatabaseContext.LetsTeamsDatabase.Find(new BsonDocument {
                    { "_id", teamID }
                }).ToList();

            var newMessage = new Message
            {
                UserName = name,
                Chat = message
            };

            if (teamById[0].Messages == null)
            {
                teamById[0].Messages = new List<Message>();
            }

            teamById[0].Messages.Add(newMessage);

            DatabaseContext.LetsTeamsDatabase.ReplaceOneAsync(r => r.Id == teamById[0].Id, teamById[0]);
        }
    }
}