namespace HashBus.Twitter.Monitor.CatchUp
{
    using System;
    using HashBus.NServiceBusConfiguration;
    using NServiceBus;
    using NServiceBus.Persistence;

    class App
    {
        public static void Run(
            int maximumNumberOfTweetsPerCatchUp,
            TimeSpan defaultTransactionTimeout,
            string nserviceBusConnectionString,
            string endpointName,
            string consumerKey,
            string consumerSecret,
            string accessToken,
            string accessTokenSecret)
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.Transactions().DoNotWrapHandlersExecutionInATransactionScope();
            busConfiguration.Transactions().DefaultTimeout(defaultTransactionTimeout);
            busConfiguration.EndpointName(endpointName);
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<NHibernatePersistence>().ConnectionString(nserviceBusConnectionString);
            busConfiguration.ApplyMessageConventions();
            busConfiguration.RegisterComponents(c=>c.RegisterSingleton<ITweetReceivedService>(
                new TweetReceivedService(maximumNumberOfTweetsPerCatchUp, consumerKey, consumerSecret, accessToken, accessTokenSecret)));

            using (Bus.Create(busConfiguration).Start())
            {
                Console.ReadLine();
            }
        }
    }
}
