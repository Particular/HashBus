namespace HashBus.Viewer.TweetLeaderboard
{
    using System.Threading.Tasks;
    using RestSharp;

    class App
    {
        public static async Task RunAsync(
            string webApiBaseUrl,
            string track,
            int refreshInterval,
            bool showPercentages,
            int verticalPadding,
            int horizontalPadding)
        {
            var client = new RestClient(webApiBaseUrl);

            await TweetLeaderboardView.StartAsync(
                track,
                refreshInterval,
                new TweetLeaderboardService(client),
                showPercentages,
                verticalPadding,
                horizontalPadding);
        }
    }
}
