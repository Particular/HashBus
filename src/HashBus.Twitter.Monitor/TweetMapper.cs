namespace HashBus.Twitter.Monitor
{
    using System;
    using System.Linq;
    using HashBus.Twitter.Events;
    using Tweetinvi.Core.Interfaces;

    static class TweetMapper
    {
        public static TweetReceived Map(ITweet tweet, string endpointName, Guid sessionId, string track)
        {
            return new TweetReceived
            {
                EndpointName = endpointName,
                SessionId = sessionId,
                Track = track,
                TweetId = tweet.Id,
                TweetCreatedAt = tweet.CreatedAt,
                TweetCreatedById = tweet.CreatedBy.Id,
                TweetCreatedByIdStr = tweet.CreatedBy.IdStr,
                TweetCreatedByName = tweet.CreatedBy.Name,
                TweetCreatedByScreenName = tweet.CreatedBy.ScreenName,
                TweetIsRetweet = tweet.IsRetweet,
                TweetText = tweet.Text,
                TweetUserMentions = tweet.UserMentions
                    .Select(userMention => new UserMention
                    {
                        Id = userMention.Id,
                        IdStr = userMention.IdStr,
                        Indices = userMention.Indices,
                        Name = userMention.Name,
                        ScreenName = userMention.ScreenName,
                    })
                    .ToArray(),
                RetweetedTweetId = tweet.IsRetweet ? tweet.RetweetedTweet.Id : default(long),
                RetweetedTweetCreatedAt = tweet.IsRetweet ? tweet.RetweetedTweet.CreatedAt : default(DateTime),
                RetweetedTweetCreatedById = tweet.IsRetweet ? tweet.RetweetedTweet.CreatedBy.Id : default(long),
                RetweetedTweetCreatedByIdStr = tweet.IsRetweet ? tweet.RetweetedTweet.CreatedBy.IdStr : default(string),
                RetweetedTweetCreatedByName = tweet.IsRetweet ? tweet.RetweetedTweet.CreatedBy.Name : default(string),
                RetweetedTweetCreatedByScreenName = tweet.IsRetweet ? tweet.RetweetedTweet.CreatedBy.ScreenName : default(string),
                TweetHashtags = tweet.Hashtags
                    .Select(ht => new Hashtag
                    {
                        Text = ht.Text,
                        Indices = ht.Indices,
                    }).ToArray(),
            };
        }
    }
}
