using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ColossalGame.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string TokenHash { get; set; }

        public DateTime TokenAge { get; set; }
    }
}
