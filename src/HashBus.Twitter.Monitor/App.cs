namespace HashBus.Twitter.Monitor
{
    using System.Threading.Tasks;
    using NServiceBus;
    using NServiceBus.Persistence;
    using Conventions;
    using System;

    class App
    {
        const string EndpointName = "HashBus.Twitter.Monitor";

        public static async Task RunAsync(string nserviceBusConnectionString, string track, string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName(EndpointName);
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<NHibernatePersistence>().ConnectionString(nserviceBusConnectionString);
            busConfiguration.ApplyMessageConventions();

            using (var bus = Bus.Create(busConfiguration).Start())
            {
                await Monitoring.StartAsync(bus, track, consumerKey, consumerSecret, accessToken, accessTokenSecret, Guid.NewGuid());
            }
        }
    }
}
