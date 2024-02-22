using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using Newtonsoft.Json;

namespace ArticlesMongoDBExercise
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Create the Database
            const string connectionUrl = "mongodb+srv://HellBunny:ZublacK3303@cluster0.fyhuw6n.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";

            var settings = MongoClientSettings.FromConnectionString(connectionUrl);

            // Set the ServerApi field of the settings object to set the version of the Stable API on the client
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            // Create a new client and connect to the server
            var client = new MongoClient(settings);

            var database = client.GetDatabase("Articles");

            var collection = database.GetCollection<BsonDocument>("articles");

            string articlesJson = File.ReadAllText("articles.json");

            var articlesData = Newtonsoft.Json.JsonConvert.DeserializeObject<ArticlesData>(articlesJson);

            var bsonArticles = articlesData.Articles.Select(articles => articles.ToBsonDocument());

            collection.InsertMany(bsonArticles);

            // Send a ping to confirm a successful connection
            try
            {
                var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            var collection2 = database.GetCollection<Article>("articles");

            // Print on the console the names of all articles in the database.

            var articleNames = IMongoCollectionExtensions.AsQueryable(collection2)
                .Select(a => a.Name)
                .ToList();
            Console.WriteLine(string.Join(Environment.NewLine, articleNames));

            // Create a new article
            var newArticle = new Article
            {
                Author = "Steve Jobs",
                Date = "05-05-2005",
                Name = "The story of Apple",
                Rating = 60
            };

            collection2.InsertOne(newArticle);

            // Update
            var collection3 = database.GetCollection<Article>("articles");
            var updateDefinition = Builders<Article>.Update.Inc(a => a.Rating, 10);
            var resultModified = collection3.UpdateMany(FilterDefinition<Article>.Empty, updateDefinition);

            Console.WriteLine($"{resultModified.ModifiedCount} documents updated successfully.");

            // Delete
            var collection4 = database.GetCollection<Article>("articles");
            collection4.DeleteMany(a => a.Rating <= 50);
            var remainingArticles = IMongoCollectionExtensions.AsQueryable(collection4)
                .Select(a => a.Name)
                .ToList();

            Console.WriteLine(string.Join(Environment.NewLine, remainingArticles));
        }
    }
}
