using System.Collections.Generic;
using System.Threading;
using HashBus.ReadModel;
using HashBus.ReadModel.MongoDB;
using NServiceBus;
using MongoDB.Driver;

namespace HashBus.Projector.MentionLeaderboard
{
    class App
    {
        public static void Run(string mongoConnectionString, string mongoDBDatabase)
        {
            var mongoDatabase = new MongoClient(mongoConnectionString).GetDatabase(mongoDBDatabase);

            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Projection.MentionLeaderboard");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.RegisterComponents(c =>
                c.RegisterSingleton<IRepository<string, IEnumerable<Mention>>>(
                    new MongoDBListRepository<Mention>(mongoDatabase, "mention_leaderboard__mentions")));

            using (Bus.Create(busConfiguration).Start())
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
