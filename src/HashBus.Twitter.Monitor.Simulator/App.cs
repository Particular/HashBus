namespace HashBus.Twitter.Monitor.Simulator
{
    using System;
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

            using (var bus = Bus.Create(busConfiguration).Start())
            {
                Simulation.Start(bus, endpointName, Guid.NewGuid());
            }
        }
    }
}
