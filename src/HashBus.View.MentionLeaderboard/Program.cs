using System.Configuration;

namespace HashBus.View.MentionLeaderboard
{
    class Program
    {
        static void Main()
        {
            var mongoConnectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
            var mongoDBDatabase = ConfigurationManager.AppSettings["MongoDBDatabase"];
            var hashTag = ConfigurationManager.AppSettings["hashTag"];
            var refreshInterval = int.Parse(ConfigurationManager.AppSettings["refreshInterval"]);

            App.RunAsync(mongoConnectionString, mongoDBDatabase, hashTag, refreshInterval).GetAwaiter().GetResult();
        }
    }
}
