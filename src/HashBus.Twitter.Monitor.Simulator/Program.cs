namespace HashBus.Twitter.Monitor.Simulator
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;

    class Program
    {
        static Task Main()
        {
            var hashtag = ConfigurationManager.AppSettings["Hashtag"];
            var nserviceBusConnectionString = ConfigurationManager.AppSettings["NServiceBusConnectionString"];
            var endpointName = ConfigurationManager.AppSettings["EndpointName"];
            var analyzerAddress = ConfigurationManager.AppSettings["AnalyzerAddress"];

            Console.Title = typeof(Program).Assembly.GetName().Name;

            return App.Run(nserviceBusConnectionString, hashtag, endpointName, analyzerAddress);
        }
    }
}
