namespace HashBus.Twitter.Monitor.CatchUp
{
    using System;
    using Conventions;
    using NServiceBus;
    using NServiceBus.Persistence;

    class App
    {
        public static void Run (string nserviceBusConnectionString)
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName("HashBus.Twitter.Monitor.CatchUp");
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<NHibernatePersistence>().ConnectionString(nserviceBusConnectionString);
            busConfiguration.ApplyMessageConventions();

            using (Bus.Create(busConfiguration).Start())
            {
                Console.ReadLine();
            }
        }
    }
}
