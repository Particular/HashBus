namespace HashBus.Projector.TopRetweeters
{
    using System;
    using System.Configuration;

    class Program
    {
        static void Main()
        {
            var mongoConnectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
            var mongoDBDatabase = ConfigurationManager.AppSettings["MongoDBDatabase"];
            var endpointName = ConfigurationManager.AppSettings["EndpointName"];

            Console.Title = typeof(Program).Assembly.GetName().Name;

            App.Run(mongoConnectionString, mongoDBDatabase, endpointName);
        }
    }
}
