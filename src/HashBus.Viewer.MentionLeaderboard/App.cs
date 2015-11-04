namespace HashBus.Viewer.MentionLeaderboard
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

            await MentionLeaderboardView.StartAsync(
                hashtag,
                refreshInterval,
                new MongoDBListRepository<Mention>(mongoDatabase, "mention_leaderboard__mentions"),
                showPercentages);
        }
    }
}
