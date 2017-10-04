namespace HashBus.Twitter.CatchUp
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using HashBus.NServiceBusConfiguration;
    using HashBus.Twitter.Analyzer.Commands;
    using HashBus.Twitter.CatchUp.Commands;
    using HashBus.Twitter.Monitor.Events;
    using NServiceBus;
    using NServiceBus.Persistence;

    class App
    {
        public static async Task Run(
            int maximumNumberOfTweetsPerCatchUp,
            TimeSpan defaultTransactionTimeout,
            string nserviceBusConnectionString,
            string endpointName,
            string consumerKey,
            string consumerSecret,
            string accessToken,
            string accessTokenSecret,
            string catchUpAddress,
            string analyzerAddress,
            string monitorAddress)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);

            var transportExtensions = endpointConfiguration.UseTransport<MsmqTransport>()
                .Transactions(TransportTransactionMode.ReceiveOnly);

            endpointConfiguration.UnitOfWork().WrapHandlersInATransactionScope(defaultTransactionTimeout);
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UsePersistence<NHibernatePersistence>().ConnectionString(nserviceBusConnectionString);
            endpointConfiguration.ApplyMessageConventions();
            endpointConfiguration.ApplyErrorAndAuditQueueSettings();
            endpointConfiguration.RegisterComponents(c=>c.RegisterSingleton<ITweetService>(
                new TweetService(maximumNumberOfTweetsPerCatchUp, consumerKey, consumerSecret, accessToken, accessTokenSecret)));
            endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);

            var routing = transportExtensions.Routing();
            routing.RouteToEndpoint(typeof(StartCatchUp), catchUpAddress);
            routing.RouteToEndpoint(typeof(AnalyzeTweet), analyzerAddress);
            routing.RegisterPublisher(typeof(TweetReceived), monitorAddress);
            
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
