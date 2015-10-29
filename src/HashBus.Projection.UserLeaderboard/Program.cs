using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus;

namespace HashBus.Projection.UserLeaderboard
{
    using System.Configuration;

    class Program
    {
        static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            if (ConfigurationManager.AppSettings["JSON_FOLDER_PATH"] == null)
            {
                throw new ArgumentException("Please make sure you have the 'JSON_FOLDER_PATH' set in your appSettings");
            }

            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Projection.UserLeaderboard");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.SendFailedMessagesTo("error");
            busConfiguration.LimitMessageProcessingConcurrencyTo(1);
            busConfiguration.RegisterComponents(c =>
                c.RegisterSingleton<IRepository<string, IEnumerable<LeaderboardProjection.Mention>>>(
                    new FileListRepository<LeaderboardProjection.Mention>(ConfigurationManager.AppSettings["JSON_FOLDER_PATH"])));

            using (await Bus.Create(busConfiguration).StartAsync())
            {
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}
