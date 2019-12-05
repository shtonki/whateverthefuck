using System;
using System.Collections.Generic;
using MongoDB.Bson;
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
            client = new MongoClient(connectionString);

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
