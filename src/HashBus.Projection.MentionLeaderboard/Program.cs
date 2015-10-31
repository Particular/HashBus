using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;

namespace HashBus.Projection.MentionLeaderboard
{
    class Program
    {
        static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
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
                    new FileListRepository<Mention>(@"C:\HashBus\MentionLeaderboardProjection.Mentions")));

            using (await Bus.Create(busConfiguration).StartAsync())
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
