    using System.Configuration;

namespace HashBus.Twitter.Monitor.Simulator
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
