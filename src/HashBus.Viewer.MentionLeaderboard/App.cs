namespace HashBus.Viewer.MentionLeaderboard
{
    using System.Threading.Tasks;
    using RestSharp;

    class App
    {
        public static async Task RunAsync(
            string webApiBaseUrl, string track, int refreshInterval, bool showPercentages)
        {
            var client = new RestClient(webApiBaseUrl);

            await MentionLeaderboardView.StartAsync(
                track,
                refreshInterval,
                new MentionLeaderboardService(client), 
                showPercentages);
        }
    }
}
