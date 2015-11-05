using System.Threading;
using NServiceBus;
using NServiceBus.Persistence;
using HashBus.Conventions;

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
            busConfiguration.UsePersistence<NHibernatePersistence>()
               .ConnectionString(@"Data Source=.\SqlExpress;Initial Catalog=NServiceBus;Integrated Security=True");
            busConfiguration.ApplyMessageConventions();

            using (Bus.Create(busConfiguration).Start())
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
