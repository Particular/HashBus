namespace HashBus.Projector.TweetLeaderboard
{
    using System.Collections.Generic;
    using System.Threading;
    using HashBus.ReadModel;
    using HashBus.ReadModel.MongoDB;
    using NServiceBus;
    using MongoDB.Driver;
    using HashBus.Conventions;

    class App
    {
        public static void Run(string mongoConnectionString, string mongoDBDatabase)
        {
            var mongoDatabase = new MongoClient(mongoConnectionString).GetDatabase(mongoDBDatabase);

            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Projection.TweetLeaderboard");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.RegisterComponents(c =>
                c.RegisterSingleton<IRepository<string, IEnumerable<Tweet>>>(
                    new MongoDBListRepository<Tweet>(mongoDatabase, "tweet_leaderboard__tweets")));
            busConfiguration.ApplyMessageConventions();

            using (Bus.Create(busConfiguration).Start())
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
