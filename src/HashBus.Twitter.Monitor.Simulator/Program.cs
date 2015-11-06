    using System.Configuration;

namespace HashBus.Twitter.Monitor.Simulator
{
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
