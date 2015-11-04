using System.Configuration;

namespace HashBus.Projector.MentionLeaderboard
{
    class Program
    {
        static void Main()
        {
            var mongoConnectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
            var mongoDBDatabase = ConfigurationManager.AppSettings["MongoDBDatabase"];

            App.RunAsync(mongoConnectionString, mongoDBDatabase).GetAwaiter().GetResult();
        }
    }
}
