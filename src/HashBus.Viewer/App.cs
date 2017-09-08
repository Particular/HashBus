namespace HashBus.Viewer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
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
            int horizontalPadding,
            string[] views,
            int rotateInterval)
        {
            var client = new RestClient(webApiBaseUrl);

            var allViews = new Dictionary<string, IRunAsync>
            {
                {
                    "MostHashtagged",
                    MostHashtaggedLeaderBoardViewFactory.Create(
                        track, refreshInterval,
                        showPercentages,
                        verticalPadding,
                        horizontalPadding,
                        client)
                },
                {
                    "MostMentioned",
                    MostMentionedLeaderBoardViewFactory.Create(
                        track, refreshInterval,
                        showPercentages,
                        verticalPadding,
                        horizontalPadding,
                        client)
                },
                {
                    "MostRetweeted",
                    MostRetweetedLeaderBoardViewFactory.Create(
                        track, refreshInterval,
                        showPercentages,
                        verticalPadding,
                        horizontalPadding,
                        client)
                },
                {
                    "TopRetweeters",
                    TopRetweetersLeaderBoardViewFactory.Create(
                        track, refreshInterval,
                        showPercentages,
                        verticalPadding,
                        horizontalPadding,
                        client)
                },
                {
                    "TopTweeters",
                    TopTweetersLeaderBoardViewFactory.Create(
                        track, refreshInterval,
                        showPercentages,
                        verticalPadding,
                        horizontalPadding,
                        client)
                },
                {
                    "TopTweetersRetweeters",
                    TopTweetersRetweetersLeaderBoardViewFactory.Create(
                        track, refreshInterval,
                        showPercentages,
                        verticalPadding,
                        horizontalPadding,
                        client)
                },
            };

            var invalidViews = views.Where(view => !allViews.ContainsKey(view)).ToList();
            if (invalidViews.Any())
            {
                throw new ArgumentException($"View(s) not found: {string.Join(", ", invalidViews)}.", nameof(views));
            }

            while (true)
            {
                foreach (var view in views)
                {
                    var cancellationTokenSource = new CancellationTokenSource();
                    var task = allViews[view].RunAsync(cancellationTokenSource.Token);
                    await Task.Delay(rotateInterval);
                    cancellationTokenSource.Cancel();
                    await task;
                }
            }
        }
    }
}
