using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HashBus.ReadModel;
using HashBus.ReadModel.MongoDB;
using NServiceBus;
using MongoDB.Driver;

namespace HashBus.Projection.UserLeaderboard
{
    class App
    {
        public static async Task RunAsync(string mongoConnectionString, string mongoDBDatabase)
        {
            var mongoDatabase = new MongoClient(mongoConnectionString).GetDatabase(mongoDBDatabase);

            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Projection.MentionLeaderboard");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.SendFailedMessagesTo("error");
            busConfiguration.LimitMessageProcessingConcurrencyTo(1);
            busConfiguration.RegisterComponents(c =>
                c.RegisterSingleton<IRepository<string, IEnumerable<Mention>>>(
                    new MongoDBListRepository<Mention>(mongoDatabase, "mention_leaderboard__mentions")));

            using (await Bus.Create(busConfiguration).StartAsync())
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
