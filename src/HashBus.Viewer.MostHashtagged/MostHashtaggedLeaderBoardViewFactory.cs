namespace HashBus.Viewer.MostHashtagged
{
    using System;
    using ColoredConsole;
    using HashBus.WebApi;
    using RestSharp;

    static class MostHashtaggedLeaderBoardViewFactory
    {
        public static LeaderboardView<HashtagEntry> Create(
            string track,
            int refreshInterval,
            bool showPercentages,
            int verticalPadding,
            int horizontalPadding,
            IRestClient client)
        {
            return new LeaderboardView<HashtagEntry>(
                track,
                refreshInterval,
                new LeaderboardService<HashtagEntry>(client, "/most-hashtagged/{0}"),
                showPercentages,
                verticalPadding,
                horizontalPadding,
                (entry1, entry2) => string.Equals(entry1.Text, entry2.Text, StringComparison.InvariantCultureIgnoreCase),
                entry => new[] { $" #{entry.Text}".Cyan(), },
                "Most Hashtagged",
                "hashtag usages");
        }
    }
}
