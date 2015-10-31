using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HashBus.ReadModel;
using NServiceBus;

namespace HashBus.Projection.UserLeaderboard
{
    class Program
    {
        const string DataFolderPathKey = "DataFolder";

        static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            if (ConfigurationManager.AppSettings[DataFolderPathKey] == null)
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
                    new FileListRepository<Mention>(Path.Combine(ConfigurationManager.AppSettings[DataFolderPathKey], "MentionLeaderboardProjection.Mentions"))));

            using (await Bus.Create(busConfiguration).StartAsync())
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
