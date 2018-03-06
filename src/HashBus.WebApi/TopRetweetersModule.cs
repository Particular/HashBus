namespace HashBus.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HashBus.ReadModel;
    using Nancy;

    public class TopRetweetersModule : NancyModule
    {
        public TopRetweetersModule
            (IRepository<string, IEnumerable<Retweet>> tweets, IIgnoredUserNamesService ignoredUserNamesService)
        {
            this.Get["/top-retweeters/{track}", true] = async (parameters, __) =>
            {
                // see https://github.com/NancyFx/Nancy/issues/1154
                var track = ((string)parameters.track).Replace("해시", "#");
                var trackTweets = (await tweets.GetAsync(track)).ToList();
                var entries = trackTweets
                    .Where(item => !ignoredUserNamesService.Get().Contains(item.UserScreenName))
                    .GroupBy(tweet => tweet.UserId)
                    .Select(g => new UserEntry
                    {
                        Id = g.Key,
                        Name = g.First().UserName,
                        ScreenName = g.First().UserScreenName,
                        Count = g.Count(),
                    })
                    .OrderByDescending(entry => entry.Count)
                    .Select((entry, index) =>
                    {
                        entry.Position = index + 1;
                        return entry;
                    })
                    .Take(10)
                    .ToList();

                return new Leaderboard<UserEntry>
                {
                    Entries = entries,
                    Count = trackTweets.Count,
                    Since = trackTweets.Any() ? trackTweets.Min(tweet => tweet.RetweetedAt) : (DateTime?)null,
                    LastActivityDateTime = trackTweets.Any() ? trackTweets.Max(tweet => tweet.RetweetedAt) : (DateTime?)null,
                };
            };
        }
    }
}
