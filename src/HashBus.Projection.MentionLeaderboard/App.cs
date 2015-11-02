using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using System.IO;

namespace HashBus.Projection.UserLeaderboard
{
    class App
    {
        public static async Task RunAsync(string dataFolder)
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Projection.MentionLeaderboard");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.SendFailedMessagesTo("error");
            busConfiguration.LimitMessageProcessingConcurrencyTo(1);
            busConfiguration.RegisterComponents(c =>
                c.RegisterSingleton<IRepository<string, IEnumerable<Mention>>>(
                    new FileListRepository<Mention>(Path.Combine(dataFolder, "MentionLeaderboardProjection.Mentions"))));

            using (await Bus.Create(busConfiguration).StartAsync())
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
