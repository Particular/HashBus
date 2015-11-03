using System.Threading.Tasks;
using NServiceBus;

namespace HashBus.Twitter.Monitor
{
    class App
    {
        public static async Task RunAsync(string hashtag, string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Twitter.Monitor");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.SendFailedMessagesTo("error");

            using (var bus = await Bus.Create(busConfiguration).StartAsync())
            {
                await Monitoring.StartAsync(bus, hashtag, consumerKey, consumerSecret, accessToken, accessTokenSecret);
            }
        }
    }
}
