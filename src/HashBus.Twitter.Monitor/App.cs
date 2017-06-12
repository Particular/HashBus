namespace HashBus.Twitter.Monitor
{
    using System.Threading.Tasks;
    using HashBus.NServiceBusConfiguration;
    using HashBus.Twitter.Analyzer.Commands;
    using NServiceBus;
    using NServiceBus.Persistence;

    class App
    {
        public static async Task RunAsync(
            string nserviceBusConnectionString,
            string track,
            string consumerKey,
            string consumerSecret,
            string accessToken,
            string accessTokenSecret,
            string endpointName,
            string analyzerAddress)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UsePersistence<NHibernatePersistence>().ConnectionString(nserviceBusConnectionString);
            endpointConfiguration.ApplyMessageConventions();
            endpointConfiguration.ApplyErrorAndAuditQueueSettings();

            var transportExtensions = endpointConfiguration.UseTransport<MsmqTransport>();

            var routing = transportExtensions.Routing();
            routing.RouteToEndpoint(typeof(AnalyzeTweet), analyzerAddress);

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            try
            {
                await Monitoring.StartAsync(
                        endpointInstance,
                        track,
                        consumerKey,
                        consumerSecret,
                        accessToken,
                        accessTokenSecret)
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
