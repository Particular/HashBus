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
            this.Get["/tweet-leaderboards/{hashtag}", true] = async (parameters, __) =>
            {
                var hashtagTweets = (await tweets.GetAsync((string)parameters.hashtag)).ToList();
                var entries = hashtagTweets
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
                    TweetsCount = hashtagTweets.Count,
                };
            };
        }
    }
}
