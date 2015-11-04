namespace HashBus.Viewer.TweetLeaderboard
{
    using System.Threading.Tasks;
    using ReadModel;
    using ReadModel.MongoDB;
    using MongoDB.Driver;

    class App
    {
        public static async Task RunAsync(
            string mongoConnectionString, string mongoDBDatabase, string hashtag, int refreshInterval, bool showPercentages)
        {
            var mongoDatabase = new MongoClient(mongoConnectionString).GetDatabase(mongoDBDatabase);

            await TweetLeaderboardView.StartAsync(
                hashtag,
                refreshInterval,
                new MongoDBListRepository<Tweet>(mongoDatabase, "tweet_leaderboard__tweets"),
                showPercentages);
        }
    }
}
