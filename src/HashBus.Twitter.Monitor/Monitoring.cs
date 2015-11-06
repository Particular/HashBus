namespace HashBus.Twitter.Monitor
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
using ColoredConsole;
    using HashBus.Twitter.Events;
    using NServiceBus;
    using Tweetinvi;
    using Tweetinvi.Core.Credentials;

    class Monitoring
    {
        const string EndpointName = "HashBus.Twitter.Monitor";

        public static async Task StartAsync(
            ISendOnlyBus bus,
            string track,
            string consumerKey,
            string consumerSecret,
            string accessToken,
            string accessTokenSecret,
			Guid sessionId)
        {
            var credentials = new TwitterCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);
            var stream = Stream.CreateFilteredStream(credentials);

            stream.AddTrack(track);

            stream.StreamStarted += (sender, args) => ColorConsole.WriteLine($" {track} ".DarkCyan().OnWhite(), " stream started.".Gray());
            stream.StreamStopped += (sender, args) => ColorConsole.WriteLine($" {track} ".DarkCyan().OnWhite(), " stream stopped.".Red(), $" {args.Exception.Message}".DarkRed());

            stream.MatchingTweetReceived += (sender, e) =>
            {
                var message = new TweetReceived
                {
                    EndpointName = EndpointName,
                    SessionId = sessionId,
                    Track = track,
                    TweetId = e.Tweet.Id,
                    TweetCreatedAt = e.Tweet.CreatedAt,
                    TweetCreatedById = e.Tweet.CreatedBy.Id,
                    TweetCreatedByIdStr = e.Tweet.CreatedBy.IdStr,
                    TweetCreatedByName = e.Tweet.CreatedBy.Name,
                    TweetCreatedByScreenName = e.Tweet.CreatedBy.ScreenName,
                    TweetIsRetweet = e.Tweet.IsRetweet,
                    TweetText = e.Tweet.Text,
                    TweetUserMentions = e.Tweet.UserMentions
                        .Select(userMention => new UserMention
                        {
                            Id = userMention.Id,
                            IdStr = userMention.IdStr,
                            Indices = userMention.Indices,
                            Name = userMention.Name,
                            ScreenName = userMention.ScreenName,
                        })
                        .ToArray(),
                    RetweetedTweetId = e.Tweet.IsRetweet ? e.Tweet.RetweetedTweet.Id : default(long),
                    RetweetedTweetCreatedAt = e.Tweet.IsRetweet ? e.Tweet.RetweetedTweet.CreatedAt : default(DateTime),
                    RetweetedTweetCreatedById = e.Tweet.IsRetweet ? e.Tweet.RetweetedTweet.CreatedBy.Id : default(long),
                    RetweetedTweetCreatedByIdStr = e.Tweet.IsRetweet ? e.Tweet.RetweetedTweet.CreatedBy.IdStr : default(string),
                    RetweetedTweetCreatedByName = e.Tweet.IsRetweet ? e.Tweet.RetweetedTweet.CreatedBy.Name : default(string),
                    RetweetedTweetCreatedByScreenName = e.Tweet.IsRetweet ? e.Tweet.RetweetedTweet.CreatedBy.ScreenName : default(string),
                    TweetHashtags = e.Tweet.Hashtags
                        .Select(ht => new Hashtag
                        {
                            Text = ht.Text,
                            Indices = ht.Indices,
                        }).ToArray(),
                };

                Writer.Write(message);
                bus.Publish(message);
            };

            await stream.StartStreamMatchingAnyConditionAsync();

            Console.ReadKey();
        }
    }
}
