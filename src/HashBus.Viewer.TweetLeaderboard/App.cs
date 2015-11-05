using System.Threading.Tasks;
using RestSharp;

namespace HashBus.Viewer.TweetLeaderboard
{
    class App
    {
        public static async Task RunAsync(
            string webApiBaseUrl, string track, int refreshInterval, bool showPercentages)
        {
            var client = new RestClient(webApiBaseUrl);

            await TweetLeaderboardView.StartAsync(
                track,
                refreshInterval,
                new TweetLeaderboardService(client),
                showPercentages);
        }
    }
}
