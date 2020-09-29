using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColossalGame.Models;
using MongoDB.Driver;

namespace ColossalGame.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("Colossal");

            _users = database.GetCollection<User>("Users");
        }

        public List<User> Get() =>
            _users.Find(user => true).ToList();

        public User Get(string id) =>
            _users.Find<User>(user => user.Id == id).FirstOrDefault();

        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }

        public void Update(string id, User userIn) =>
            _users.ReplaceOne(user => user.Id == id, userIn);

        public void Remove(User userIn) =>
            _users.DeleteOne(user => user.Id == userIn.Id);

        public void Remove(string id) =>
            _users.DeleteOne(user => user.Id == id);

        public User GetByUsername(string username)
        {
            return _users.Find<User>(user => user.Username == username).FirstOrDefault();
        }

        public bool UserExistsByUsername(string username)
        {
            
            return _users.Find<User>(user => user.Username == username).FirstOrDefault()!=null;
        }

        public string generateToken(User userIn)
        {
            Guid g = Guid.NewGuid();
            string guidString = Convert.ToBase64String(g.ToByteArray());
            userIn.TokenHash = BCrypt.Net.BCrypt.HashPassword(guidString);
            userIn.TokenAge = DateTime.Now;
            Update(userIn.Id,userIn);
            return guidString;

        }
    }
}

