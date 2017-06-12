namespace HashBus.Twitter.BackFill
{
    using System.Threading.Tasks;
    using HashBus.NServiceBusConfiguration;
    using HashBus.Twitter.CatchUp.Commands;
    using NServiceBus;
    using NServiceBus.Persistence;

    class App
    {
        public static async Task Run(
            string nserviceBusConnectionString,
            string endpointName,
            string track,
            long tweetId,
            string catchUpAddress)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UsePersistence<NHibernatePersistence>().ConnectionString(nserviceBusConnectionString);
            endpointConfiguration.ApplyMessageConventions();
            endpointConfiguration.ApplyErrorAndAuditQueueSettings();

            var transportExtensions = endpointConfiguration.UseTransport<MsmqTransport>();

            var routing = transportExtensions.Routing();
            routing.RouteToEndpoint(typeof(StartCatchUp), catchUpAddress);

            var command = new StartCatchUp
            {
                EndpointName = endpointName,
                Track = track,
                TweetId = tweetId,
            };

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            try
            {

                await endpointInstance.Send(command)
                    .ConfigureAwait(false);
            }
            finally
            {
                await endpointInstance.Stop()
                    .ConfigureAwait(false);
            }
        }
    }
}
