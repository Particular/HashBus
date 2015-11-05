using System.Configuration;

namespace HashBus.Application
{
    class Program
    {
        static void Main()
        {
            var nserviceBusConnectionString = ConfigurationManager.AppSettings["NServiceBusConnectionString"];
            App.Run(nserviceBusConnectionString);
        }
    }
}
