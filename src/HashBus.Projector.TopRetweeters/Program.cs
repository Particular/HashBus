namespace HashBus.Projector.TopRetweeters
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;

    class Program
    {
        static Task Main()
        {
            var mongoConnectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
            var mongoDBDatabase = ConfigurationManager.AppSettings["MongoDBDatabase"];
            var endpointName = ConfigurationManager.AppSettings["EndpointName"];
            var analyzerAddress = ConfigurationManager.AppSettings["AnalyzerAddress"];

            Console.Title = typeof(Program).Assembly.GetName().Name;

            return App.Run(mongoConnectionString, mongoDBDatabase, endpointName, analyzerAddress);
        }
    }
}
