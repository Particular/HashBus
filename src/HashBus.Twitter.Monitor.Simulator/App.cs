using NServiceBus;

namespace HashBus.Twitter.Monitor.Simulator
{
    class App
    {
        public static void Run()
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Twitter.Monitor");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();

            using (var bus = Bus.Create(busConfiguration).Start())
            {
                Simulation.Start(bus);
            }
        }
    }
}
