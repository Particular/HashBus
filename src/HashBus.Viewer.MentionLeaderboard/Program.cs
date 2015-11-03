using System.Configuration;

namespace HashBus.Viewer.MentionLeaderboard
{
    class Program
    {
        static void Main()
        {
            var mongoConnectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
            var mongoDBDatabase = ConfigurationManager.AppSettings["MongoDBDatabase"];
            var hashTag = ConfigurationManager.AppSettings["hashTag"];
            var refreshInterval = int.Parse(ConfigurationManager.AppSettings["refreshInterval"]);
            var showPercentages = bool.Parse(ConfigurationManager.AppSettings["ShowPercentages"]);

            App.RunAsync(mongoConnectionString, mongoDBDatabase, hashTag, refreshInterval, showPercentages).GetAwaiter().GetResult();
        }
    }
}
