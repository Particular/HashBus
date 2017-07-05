namespace HashBus.Application
{
    using System;
    using System.Configuration;

    class Program
    {
        static void Main()
        {
            var nserviceBusConnectionString = ConfigurationManager.AppSettings["NServiceBusConnectionString"];
            var endpointName = ConfigurationManager.AppSettings["EndpointName"];

            Console.Title = typeof(Program).Assembly.GetName().Name;

            App.Run(nserviceBusConnectionString, endpointName);
        }
    }
}
