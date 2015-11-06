namespace HashBus.Application
{
    using System.Threading;
    using HashBus.NServiceBusConfiguration;
    using NServiceBus;
    using NServiceBus.Persistence;

    class App
    {
        public static void Run(string nserviceBusConnectionString)
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Application");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<NHibernatePersistence>().ConnectionString(nserviceBusConnectionString);
            busConfiguration.ApplyMessageConventions();

            using (Bus.Create(busConfiguration).Start())
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
