namespace HashBus.Viewer.MostHashtagged
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

            await LeaderboardView<HashtagEntry>.StartAsync(
                track,
                refreshInterval,
                new LeaderboardService<HashtagEntry>(client, "/most-hashtagged/{0}"),
                showPercentages,
                verticalPadding,
                horizontalPadding,
                (entry1, entry2) => string.Equals(entry1.Text, entry2.Text, System.StringComparison.InvariantCultureIgnoreCase),
                entry => new[] { $" #{entry.Text}".Cyan(), },
                "Most Hashtagged",
                "hashtag usages");

        }
    }
}
