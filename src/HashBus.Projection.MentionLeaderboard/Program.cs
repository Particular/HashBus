using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using System;
using System.Configuration;
using System.IO;

namespace HashBus.Projection.UserLeaderboard
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
                c.RegisterSingleton<IRepository<string, IEnumerable<LeaderboardProjection.Mention>>>(
                    new FileListRepository<LeaderboardProjection.Mention>(Path.Combine(ConfigurationManager.AppSettings[DataFolderPath], "LeaderboardProjection.Mention"))));

            using (await Bus.Create(busConfiguration).StartAsync())
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
