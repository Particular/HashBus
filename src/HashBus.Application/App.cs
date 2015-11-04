using System.Threading;
using NServiceBus;

namespace HashBus.Application
{
    class App
    {
        public static void Run()
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Application");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();

            using (Bus.Create(busConfiguration).Start())
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
