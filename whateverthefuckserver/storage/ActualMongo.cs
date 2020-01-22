using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using whateverthefuck.src.model;
using whateverthefuck.src.util;

namespace whateverthefuckserver.storage
{
    class ActualMongo : IActualDatabase
    {
        private MongoClient client;
        private const string DB_NAME = "Wetf";

        public ActualMongo()
        {
          /* UserInfos = new Dictionary<string, UserInfo>();

           if (File.Exists(FilePath))
           {
               var bs = File.ReadAllBytes(FilePath);
               if (bs != null && bs.Length > 0)
               {
                   try
                   {
                       Read(bs);
                       Logging.Log("Dank user database");
                   }
                   catch (Exception e)
                   {
                       Logging.Log("Corrupt user database");
                   }
               }
           }
           */
          var connectionString = "localhost";
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
#pragma warning disable CS0618 // Type or member is obsolete
                AutoIndexId = false,
#pragma warning restore CS0618 // Type or member is obsolete
            });
        }

        public void AddBson(string collectionName, object o)
        {
            var document = o.ToBsonDocument();
            var collection = client.GetDatabase(DB_NAME).GetCollection<BsonDocument>(collectionName);
            collection.InsertOne(document);
            Logging.Log("Added bson entry to db", Logging.LoggingLevel.Info);
        }

        public ItemStorable LoadBson(string collectionName, Expression<Func<BsonDocument, bool>> filter)
        {
            var collection = client.GetDatabase(DB_NAME).GetCollection<BsonDocument>(collectionName);
            BsonValue bv = null;
            var xdd = collection.FindSync<ItemStorable>(filter);
            xdd.MoveNext();

            return xdd.Current.ElementAt(0);
        }

        public void StoreUserInfo(UserInfo info)
        {
            throw new NotImplementedException();
        }

        public UserInfo GetUserInfo(string username)
        {
            throw new NotImplementedException();
        }
    }
}
