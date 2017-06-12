namespace HashBus.Twitter.Analyzer
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;

    class Program
    {
        static Task Main()
        {
            var nserviceBusConnectionString = ConfigurationManager.AppSettings["NServiceBusConnectionString"];
            var endpointName = ConfigurationManager.AppSettings["EndpointName"];

            Console.Title = typeof(Program).Assembly.GetName().Name;

            return App.Run(nserviceBusConnectionString, endpointName);
        }
    }
}
