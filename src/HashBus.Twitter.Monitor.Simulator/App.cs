using NServiceBus;
using NServiceBus.Persistence;
using HashBus.Conventions;

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
            busConfiguration.UsePersistence<NHibernatePersistence>()
                           .ConnectionString(@"Data Source=.\SqlExpress;Initial Catalog=NServiceBus;Integrated Security=True");
            busConfiguration.ApplyMessageConventions();

            using (var bus = Bus.Create(busConfiguration).Start())
            {
                Simulation.Start(bus);
            }
        }
    }
}
