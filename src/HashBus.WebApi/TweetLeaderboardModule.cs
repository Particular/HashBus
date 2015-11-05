using System.Collections.Generic;
using System.Linq;
using HashBus.ReadModel;
using Nancy;

namespace HashBus.WebApi
{
    public class TweetLeaderboardModule : NancyModule
    {
        public TweetLeaderboardModule(IRepository<string, IEnumerable<Tweet>> tweets)
        {
            this.Get["/tweet-leaderboards/{track}", true] = async (parameters, __) =>
            {
                // see https://github.com/NancyFx/Nancy/issues/1154
                var track = ((string)parameters.track).Replace("해시", "#");
                var trackTweets = (await tweets.GetAsync(track)).ToList();
                var entries = trackTweets
                    .GroupBy(tweet => tweet.UserId)
                    .Select(g => new TweetLeaderboard.Entry
                    {
                        UserId = g.Key,
                        UserIdStr = g.First().UserIdStr,
                        UserName = g.First().UserName,
                        UserScreenName = g.First().UserScreenName,
                        Count = g.Count(),
                    })
                    .OrderByDescending(entry => entry.Count)
                    .Take(10)
                    .ToList();

                return new TweetLeaderboard
                {
                    Entries = entries,
                    TweetsCount = trackTweets.Count,
                };
            };
        }
    }
}
