namespace HashBus.Twitter.Analyzer
{
    using System.Threading;
    using System.Threading.Tasks;
    using HashBus.NServiceBusConfiguration;
    using NServiceBus;
    using NServiceBus.Persistence;

    class App
    {
        public static async Task Run(string nserviceBusConnectionString, string endpointName)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UsePersistence<NHibernatePersistence>().ConnectionString(nserviceBusConnectionString);
            endpointConfiguration.ApplyMessageConventions();
            endpointConfiguration.ApplyErrorAndAuditQueueSettings();
            endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            try
            {
                await Task.Delay(Timeout.Infinite);
            }
            finally
            {
                await endpointInstance.Stop()
                    .ConfigureAwait(false);
            }
        }
    }
}
