using System.Threading.Tasks;
using HashBus.ReadModel;
using HashBus.ReadModel.MongoDB;
using MongoDB.Driver;

namespace HashBus.View.TweetLeaderboard
{
    class App
    {
        public static async Task RunAsync(
            string mongoConnectionString, string mongoDBDatabase, string hashtag, int refreshInterval, bool showPercentages)
        {
            var mongoDatabase = new MongoClient(mongoConnectionString).GetDatabase(mongoDBDatabase);

            await View.StartAsync(
                hashtag,
                refreshInterval,
                new MongoDBListRepository<Tweet>(mongoDatabase, "tweet_leaderboard__tweets"),
                showPercentages);
        }
    }
}
