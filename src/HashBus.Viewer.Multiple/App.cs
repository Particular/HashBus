namespace HashBus.Viewer.TopTweetersRetweeters
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
            int horizontalPadding,
            int rotateInterval)
        {
            var client = new RestClient(webApiBaseUrl);

            var view1 = new LeaderboardView<UserEntry> { Visible = false };
            var task = view1.StartAsync(
                    track,
                    refreshInterval,
                    new LeaderboardService<UserEntry>(client, "/top-tweeters-retweeters/{0}"),
                    showPercentages,
                    verticalPadding,
                    horizontalPadding,
                    (entry1, entry2) => entry1.Id == entry2.Id,
                    entry => new[] { $" {entry.Name}".White(), $" @{entry.ScreenName}".Cyan(), },
                    "Top Tweeters/Retweeters",
                    "tweets/retweets");

            var view2 = new LeaderboardView<HashtagEntry> { Visible = false };
            task = view2.StartAsync(
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

            var view3 = new LeaderboardView<UserEntry> { Visible = false };
            task = view3.StartAsync(
                track,
                refreshInterval,
                new LeaderboardService<UserEntry>(client, "/most-mentioned/{0}"),
                showPercentages,
                verticalPadding,
                horizontalPadding,
                (entry1, entry2) => entry1.Id == entry2.Id,
                entry => new[] { $" {entry.Name}".White(), $" @{entry.ScreenName}".Cyan(), },
                "Most Mentioned",
                "mentions");

            while (true)
            {
                view1.Visible = true;
                view2.Visible = false;
                view3.Visible = false;

                await Task.Delay(rotateInterval);

                view1.Visible = false;
                view2.Visible = true;
                view3.Visible = false;

                await Task.Delay(rotateInterval);

                view1.Visible = false;
                view2.Visible = false;
                view3.Visible = true;

                await Task.Delay(rotateInterval);
            }
        }
    }
}
