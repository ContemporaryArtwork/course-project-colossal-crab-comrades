using System;
using ColossalGame.Models;
using ColossalGame.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using NUnit.Framework;

namespace ColossalServiceTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestMongoIsUp()
        {
            try
            {
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                
                var database = client.GetDatabase("Colossal");

                database.RunCommandAsync((Command<BsonDocument>)"{ping:1}")
                    .Wait();
            }
            catch (Exception)
            {
                Assert.Fail("The MongoDB server is not running.");
            }
            
        }

        [Test]
        public void TestMongoServiceAdd()
        {
            try
            {
                UserService us = new UserService();
                var x = us.Create(new User{PasswordHash = "test",Username = "test"});
                var y = us.Get(x.Id);
                //Console.WriteLine(y.Id);
                Assert.AreEqual("test",y.Username);

            }
            catch (Exception e)
            {
                Assert.Fail(e.StackTrace);
            }
            

        }

    }
}