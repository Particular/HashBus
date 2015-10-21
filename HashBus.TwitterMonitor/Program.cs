namespace HashBus.TwitterMonitor
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;
    using NServiceBus;
    using Tweetinvi;
    using Tweetinvi.Core.Credentials;

    class Program
    {
        static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Server");
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
                Console.WriteLine("Tweet received with ID {0}", e.Tweet.Id);

                var message = new HashtagTweeted
                {
                    Id = e.Tweet.Id,
                };

                bus.PublishAsync(message).GetAwaiter().GetResult();
            };

            await stream.StartStreamMatchingAnyConditionAsync();

            Console.ReadKey();
        }
    }
}
