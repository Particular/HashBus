namespace HashBus.Twitter.BackFill
{
    using HashBus.NServiceBusConfiguration;
    using HashBus.Twitter.Monitor.Commands;
    using NServiceBus;
    using NServiceBus.Persistence;

    class App
    {
        public static void Run(
            string nserviceBusConnectionString,
            string endpointName,
            string track,
            long tweetId)
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName(endpointName);
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<NHibernatePersistence>().ConnectionString(nserviceBusConnectionString);
            busConfiguration.ApplyMessageConventions();

            using (var bus = Bus.Create(busConfiguration).Start())
            {
                var command = new StartCatchUp
                {
                    EndpointName = endpointName,
                    Track = track,
                    TweetId = tweetId,
                };

                bus.Send(command);
            }
        }
    }
}
