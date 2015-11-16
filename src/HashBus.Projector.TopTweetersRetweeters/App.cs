namespace HashBus.Projector.TopTweetersRetweeters
{
    using System.Collections.Generic;
    using System.Threading;
    using HashBus.NServiceBusConfiguration;
    using HashBus.ReadModel;
    using HashBus.ReadModel.MongoDB;
    using MongoDB.Driver;
    using NServiceBus;

    class App
    {
        public static void Run(string mongoConnectionString, string mongoDBDatabase)
        {
            var mongoDatabase = new MongoClient(mongoConnectionString).GetDatabase(mongoDBDatabase);

            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Projector.TopTweetersRetweeters");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.RegisterComponents(c =>
                c.RegisterSingleton<IRepository<string, IEnumerable<TweetRetweet>>>(
                    new MongoDBListRepository<TweetRetweet>(mongoDatabase, "top_tweeters_retweeters__tweet_retweets")));
            busConfiguration.ApplyMessageConventions();

            using (Bus.Create(busConfiguration).Start())
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
