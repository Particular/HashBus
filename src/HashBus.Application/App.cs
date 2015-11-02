using System.Threading;
using System.Threading.Tasks;
using NServiceBus;

namespace HashBus.Application
{
    class App
    {
        public static async Task RunAsync()
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Application");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.SendFailedMessagesTo("error");

            using (await Bus.Create(busConfiguration).StartAsync())
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
