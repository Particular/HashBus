﻿namespace HashBus.Viewer
{
    using ColoredConsole;
    using HashBus.WebApi;
    using RestSharp;

    static class MostMentionedLeaderBoardViewFactory
    {
        public static LeaderboardView<UserEntry> Create(
            string track,
            int refreshInterval,
            bool showPercentages,
            int verticalPadding,
            int horizontalPadding,
            IRestClient client)
        {
            return new LeaderboardView<UserEntry>(
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
        }
    }
}
