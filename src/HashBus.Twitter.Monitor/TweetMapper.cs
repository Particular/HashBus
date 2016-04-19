namespace HashBus.Twitter.Monitor
{
    using System.Linq;
    using HashBus.Application.Commands;
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
                    Track = track,
                    Id = tweet.Id,
                    CreatedAt = tweet.CreatedAt,
                    CreatedById = tweet.CreatedBy.Id,
                    CreatedByIdStr = tweet.CreatedBy.IdStr,
                    CreatedByName = tweet.CreatedBy.Name,
                    CreatedByScreenName = tweet.CreatedBy.ScreenName,
                    Text = tweet.Text,
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
                    Hashtags = tweet.Hashtags
                        .Select(hashtag =>
                            new Hashtag
                            {
                                Text = hashtag.Text,
                                Indices = hashtag.Indices,
                            })
                        .ToList(),
                    RetweetedTweet = MapTweet(tweet.RetweetedTweet, track),
                };
        }
    }
}
