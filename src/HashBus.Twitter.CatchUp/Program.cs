namespace HashBus.Twitter.Monitor.CatchUp
{
    using System.Configuration;

    class Program
    {
        static void Main()
        {
            var nserviceBusConnectionString = ConfigurationManager.AppSettings["NServiceBusConnectionString"];
            var endpointName = ConfigurationManager.AppSettings["EndpointName"];
            App.Run(nserviceBusConnectionString, endpointName);
        }
    }
}
