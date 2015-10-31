using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using System;
using System.Configuration;

namespace HashBus.Projection.MentionLeaderboard
{
    class Program
    {
        const string DataFolderPath = "DataFolder";

        static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            if (ConfigurationManager.AppSettings[DataFolderPath] == null)
            {
                throw new ArgumentException("Please make sure you have the 'DataFolder' set in your appSettings");
            }

            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Projection.MentionLeaderboard");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.SendFailedMessagesTo("error");
            busConfiguration.LimitMessageProcessingConcurrencyTo(1);
            busConfiguration.RegisterComponents(c =>
                c.RegisterSingleton<IRepository<string, IEnumerable<Mention>>>(
                    new FileListRepository<LeaderboardProjection.Mention>(ConfigurationManager.AppSettings[DataFolderPath])));

            using (await Bus.Create(busConfiguration).StartAsync())
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
