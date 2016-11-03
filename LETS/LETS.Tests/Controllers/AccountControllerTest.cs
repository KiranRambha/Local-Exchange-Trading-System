using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using LETS.Models;
using MongoDB.Bson;

namespace LETS.Tests.Controllers
{
    public class RegisterUserTests : AssertionHelper
    {
        [Test]
        public void ToDocument_UserWithAnId_IdRepresentedAsAnObjectId()
        {
            var user = new RegisterUserViewModel();
            user.PersonId = ObjectId.GenerateNewId().ToString();

            var document = user.ToBsonDocument();
            Expect(document["_id"].BsonType, Is.EqualTo(BsonType.ObjectId));
        }
    }
}
