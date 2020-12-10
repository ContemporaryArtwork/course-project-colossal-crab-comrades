using ColossalGame.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Core.Configuration;

namespace ColossalGame.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(string connectionString = "mongodb://db:27017/")
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("Colossal");

            _users = database.GetCollection<User>("Users");


            database.RunCommandAsync((Command<BsonDocument>)"{ping:1}")
                .Wait();
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

            return GetByUsername(username) != null;
        }

        public string generateToken(User userIn)
        {
            var rand = new Random();
            var bytes = new byte[32];
            rand.NextBytes(bytes);
            string tokenString = Convert.ToBase64String(bytes);
            userIn.TokenHash = BCrypt.Net.BCrypt.HashPassword(tokenString,4);
            userIn.TokenAge = DateTime.Now;
            Update(userIn.Id, userIn);
            return tokenString;

        }
    }
}

