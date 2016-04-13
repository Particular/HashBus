namespace HashBus.Twitter.Monitor
{
    using System;
    using System.Linq;
    using HashBus.Application.Events;
    using HashBus.Twitter.Monitor.Events;
    using Tweetinvi.Core.Interfaces;

    static class TweetMapper
    {
        public static TweetReceived Map(ITweet tweet, string track, string endpointName, Guid sessionId)
        {
            return new TweetReceived
            {
                EndpointName = endpointName,
                SessionId = sessionId,
                Tweet = Map(tweet, track)
            };
        }

        private static Tweet Map(ITweet tweet, string track)
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
                    RetweetedTweet = Map(tweet.RetweetedTweet, track),
                };
        }
    }
}
