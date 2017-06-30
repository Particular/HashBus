namespace HashBus.Projector.MostRetweeted
{
    using System.Collections.Generic;
    using System.Threading;
    using HashBus.NServiceBusConfiguration;
    using HashBus.ReadModel;
    using HashBus.ReadModel.MongoDB;
    using MongoDB.Driver;
    using NServiceBus;

    class App
    {
        public static void Run(string mongoConnectionString, string mongoDBDatabase, string endpointName)
        {
            var mongoDatabase = new MongoClient(mongoConnectionString).GetDatabase(mongoDBDatabase);

            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName(endpointName);
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.EnableInstallers();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.RegisterComponents(c =>
                c.RegisterSingleton<IRepository<string, IEnumerable<Retweetee>>>(
                    new MongoDBListRepository<Retweetee>(mongoDatabase, "most_retweeted__retweetees")));
            busConfiguration.ApplyMessageConventions();

            using (Bus.Create(busConfiguration).Start())
            {
                Thread.Sleep(Timeout.Infinite);
            }
        }
    }
}
