namespace HashBus.Twitter.Analyzer
{
    using System.Threading;
    using HashBus.NServiceBusConfiguration;
    using NServiceBus;
    using NServiceBus.Persistence;

    class App
    {
        public static void Run(string nserviceBusConnectionString, string endpointName)
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName(endpointName);
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
