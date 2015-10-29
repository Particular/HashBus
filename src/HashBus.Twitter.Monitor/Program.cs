using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using HashBus.Twitter.Events;
using NServiceBus;
using Tweetinvi;
using Tweetinvi.Core.Credentials;

namespace HashBus.Twitter.Monitor
{
    class Program
    {
        static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Twitter.Monitor");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.SendFailedMessagesTo("error");

            using (var bus = await Bus.Create(busConfiguration).StartAsync())
            {
                await MonitorTwitter(
                    bus,
                    ConfigurationManager.AppSettings["consumerKey"],
                    ConfigurationManager.AppSettings["consumerSecret"],
                    ConfigurationManager.AppSettings["accessToken"],
                    ConfigurationManager.AppSettings["accessTokenSecret"],
                    ConfigurationManager.AppSettings["hashTag"]);
            }
        }

        static async Task MonitorTwitter(ISendOnlyBus bus, string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret, string hashtag)
        {
            var credentials = new TwitterCredentials(
                consumerKey,
                consumerSecret,
                accessToken,
                accessTokenSecret);

            var stream = Stream.CreateFilteredStream(credentials);

            stream.AddTrack('#' + hashtag);

            stream.StreamStarted += (sender, args) => Console.WriteLine($"\"{hashtag}\" stream started.");
            stream.StreamStopped += (sender, args) => Console.WriteLine($"\"{hashtag}\" stream stopped. {args.Exception}");
            stream.MatchingTweetReceived += (sender, e) =>
            {
                var message = new HashtagTweeted
                {
                    Hashtag = hashtag,
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
                bus.PublishAsync(message).GetAwaiter().GetResult();
            };

            await stream.StartStreamMatchingAnyConditionAsync();

            Console.ReadKey();
        }
    }
}
