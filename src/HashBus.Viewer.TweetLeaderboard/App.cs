namespace HashBus.Viewer.TweetLeaderboard
{
    using System.Threading.Tasks;
using RestSharp;

    class App
    {
        public static async Task RunAsync(
            string webApiBaseUrl, string hashtag, int refreshInterval, bool showPercentages)
        {
            var client = new RestClient(webApiBaseUrl);

            await TweetLeaderboardView.StartAsync(
                hashtag,
                refreshInterval,
                new TweetLeaderboardService(client),
                showPercentages);
        }
    }
}
