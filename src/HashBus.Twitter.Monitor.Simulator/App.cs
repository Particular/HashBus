namespace HashBus.Twitter.Monitor.Simulator
{
    using NServiceBus;
    using NServiceBus.Persistence;
    using HashBus.Conventions;
    using System;

    class App
    {
        const string EndpointName = "HashBus.Twitter.Monitor";

        public static void Run(string nserviceBusConnectionString)
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName(EndpointName);
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<NHibernatePersistence>().ConnectionString(nserviceBusConnectionString);
            busConfiguration.ApplyMessageConventions();

            using (var bus = Bus.Create(busConfiguration).Start())
            {
                var sessionId = Guid.NewGuid();

                Simulation.Start(bus, EndpointName, sessionId);
            }
        }
    }
}
