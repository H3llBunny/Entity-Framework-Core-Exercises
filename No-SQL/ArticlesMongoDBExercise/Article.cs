
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ArticlesMongoDBExercise
{
    public class Article
    {
        [BsonElement("author")]
        public string Author { get; set; }

        [BsonElement("date")]
        public string Date { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("rating")]
        public int Rating { get; set; }
    }

    public class ArticlesData
    {
        [BsonElement("articles")]
        public List<Article> Articles { get; set; }
    }
}
