using System.Threading.Tasks;
using NServiceBus;

namespace HashBus.Twitter.Monitor.Simulator
{
    class App
    {
        public static async Task RunAsync()
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Twitter.Monitor");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.SendFailedMessagesTo("error");

            using (var bus = await Bus.Create(busConfiguration).StartAsync())
            {
                await Simulator.StartAsync(bus);
            }
        }
    }
}
