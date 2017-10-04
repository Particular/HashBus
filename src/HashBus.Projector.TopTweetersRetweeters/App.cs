namespace HashBus.Projector.TopTweetersRetweeters
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using HashBus.NServiceBusConfiguration;
    using HashBus.ReadModel;
    using HashBus.ReadModel.MongoDB;
    using MongoDB.Driver;
    using NServiceBus;

    class App
    {
        public static async Task Run(string mongoConnectionString, string mongoDBDatabase, string endpointName, string analyzerAddress)
        {
            var mongoDatabase = new MongoClient(mongoConnectionString).GetDatabase(mongoDBDatabase);

            var endpointConfiguration = new EndpointConfiguration(endpointName);
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.RegisterComponents(c =>
                c.RegisterSingleton<IRepository<string, IEnumerable<TweetRetweet>>>(
                    new MongoDBListRepository<TweetRetweet>(mongoDatabase, "top_tweeters_retweeters__tweets_retweets")));
            endpointConfiguration.ApplyMessageConventions();
            endpointConfiguration.ApplyErrorAndAuditQueueSettings();
            endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);

            var transportExtensions = endpointConfiguration.UseTransport<MsmqTransport>();

            var routing = transportExtensions.Routing();
            routing.RegisterPublisher(typeof(Twitter.Analyzer.Events.TweetAnalyzed), analyzerAddress);

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
