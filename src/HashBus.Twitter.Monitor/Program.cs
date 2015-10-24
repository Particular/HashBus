using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ColoredConsole;
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
                    Id = e.Tweet.Id,
                    Hashtag = hashtag,
                    IsRetweet = e.Tweet.IsRetweet,
                    Text = e.Tweet.Text,
                    UserId = e.Tweet.CreatedBy.Id,
                    UserName = e.Tweet.CreatedBy.Name,
                    UserScreenName = e.Tweet.CreatedBy.ScreenName,
                    CreatedAt = e.Tweet.CreatedAt,
                    UserMentions = e.Tweet.UserMentions
                        .Select(userMention => new UserMention
                        {
                            Id = userMention.Id,
                            IdStr = userMention.IdStr,
                            Indices = userMention.Indices,
                            Name = userMention.Name,
                            ScreenName = userMention.ScreenName,
                        })
                        .ToArray()
                };

                ColorConsole.WriteLine(
                    $"{message.CreatedAt} ".DarkCyan(),
                    message.IsRetweet ? "Retweet by ".DarkGreen() : "Tweet by ".Green(),
                    $"{message.UserName} ".Yellow(),
                    $"@{message.UserScreenName}".DarkYellow());

                ColorConsole.WriteLine($"  {message.Text}".White());

                bus.PublishAsync(message).GetAwaiter().GetResult();
            };

            await stream.StartStreamMatchingAnyConditionAsync();

            Console.ReadKey();
        }
    }
}
