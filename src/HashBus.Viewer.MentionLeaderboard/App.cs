namespace HashBus.Viewer.MentionLeaderboard
{
    using System.Threading.Tasks;
    using ColoredConsole;
    using HashBus.WebApi;
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

            await LeaderboardView<UserEntry>.StartAsync(
                track,
                refreshInterval,
                new MentionLeaderboardService(client),
                showPercentages,
                verticalPadding,
                horizontalPadding,
                (entry1, entry2) => entry1.Id == entry2.Id,
                entry => new[] { $" {entry.Name}".White(), $" @{entry.ScreenName}".Cyan(), },
                "Most mentioned",
                "mentions");
        }
    }
}
