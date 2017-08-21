namespace HashBus.Twitter
{
    using System.Linq;
    using HashBus.Twitter.Analyzer.Commands;
    using Tweetinvi.Core.Interfaces;

    static class TweetMapper
    {
        public static AnalyzeTweet Map(ITweet tweet, string track)
        {
            return new AnalyzeTweet
            {
                Tweet = MapTweet(tweet, track)
            };
        }

        private static Tweet MapTweet(ITweet tweet, string track)
        {
            return tweet == null
                ? null
                : new Tweet
                {
                    CreatedAt = tweet.CreatedAt,
                    CreatedById = tweet.CreatedBy.Id,
                    CreatedByIdStr = tweet.CreatedBy.IdStr,
                    CreatedByName = tweet.CreatedBy.Name,
                    CreatedByScreenName = tweet.CreatedBy.ScreenName,
                    Hashtags = tweet.Hashtags
                        .Select(hashtag =>
                            new Hashtag
                            {
                                Text = hashtag.Text,
                                Indices = hashtag.Indices,
                            })
                        .ToList(),
                    Id = tweet.Id,
                    RetweetedTweet = MapTweet(tweet.RetweetedTweet, track),
                    Text = tweet.Text,
                    Track = track,
                    UserMentions = tweet.UserMentions
                        .Where(userMention => userMention.Id.HasValue)
                        .Select(userMention =>
                            new UserMention
                            {
                                Id = userMention.Id.Value,
                                IdStr = userMention.IdStr,
                                Indices = userMention.Indices,
                                Name = userMention.Name,
                                ScreenName = userMention.ScreenName,
                            })
                        .ToList(),
                };
        }
    }
}
