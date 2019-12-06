using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using whateverthefuck.src.util;

namespace whateverthefuckserver.storage
{
    class Mongo : IStorage
    {
        private MongoClient client;
        private const string DB_NAME = "Wetf";

        public Mongo()
        {
            var connectionString = "mongodb://localhost:27017";
            client = new MongoClient(new MongoClientSettings { Server = new MongoServerAddress(connectionString), SocketTimeout = new TimeSpan(0, 0, 0, 2), WaitQueueTimeout = new TimeSpan(0, 0, 0, 2), ConnectTimeout = new TimeSpan(0, 0, 0, 2) });

            var dbList = client.ListDatabases().ToList();
            Logging.Log("The list of databases on this server is: ");
            foreach (var db in dbList)
            {
                Console.WriteLine(db);
            }
        }

        public void CreateCollection(string collectionName)
        {
            var db = client.GetDatabase(DB_NAME);
            db.CreateCollection(collectionName, new CreateCollectionOptions
            {
                AutoIndexId = false,
            });
        }

        public void AddJson(string collectionName, object o)
        {
            // Todo: We are converting to json then deserializing it to bson.

            string json = JsonIO.ConvertToJson(o);
            var document = BsonSerializer.Deserialize<BsonDocument>(json);
            var collection = client.GetDatabase(DB_NAME).GetCollection<BsonDocument>(collectionName);
            collection.InsertOne(document);
            Console.WriteLine(json);
            Logging.Log("Added json entry to db", Logging.LoggingLevel.Info);
        }

        public void AddBson(string collectionName, object o)
        {
            var document = o.ToBsonDocument();
            var collection = client.GetDatabase(DB_NAME).GetCollection<BsonDocument>(collectionName);
            collection.InsertOne(document);
            Console.WriteLine("lmao");
            Logging.Log("Added bson entry to db", Logging.LoggingLevel.Info);
        }

        public void AddEntry(string collectionName, IStorable entry)
        {
            var collection = client.GetDatabase(DB_NAME).GetCollection<BsonDocument>(collectionName);
            var document = new BsonDocument();

            document.Add("name", "Seba");
            document.Add("age", 26);
            document.Add("dick", new BsonArray() { "yes", "", "" });

            collection.InsertOne(document);
            Console.WriteLine("added entry");
        }
    }
}
