namespace HashBus.Twitter.Monitor.Simulator
{
    using System;
    using System.Configuration;

    class Program
    {
        static void Main()
        {
            var hashtag = ConfigurationManager.AppSettings["Hashtag"];
            var nserviceBusConnectionString = ConfigurationManager.AppSettings["NServiceBusConnectionString"];
            var endpointName = ConfigurationManager.AppSettings["EndpointName"];

            Console.Title = typeof(Program).Assembly.GetName().Name;

            App.Run(nserviceBusConnectionString, hashtag, endpointName);
        }
    }
}
