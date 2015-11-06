namespace HashBus.Twitter.Monitor.CatchUp
{
    using System.Configuration;

    class Program
    {
        static void Main()
        {
            var nserviceBusConnectionString = ConfigurationManager.ConnectionStrings["NServiceBusConnectionString"].ConnectionString;

            App.Run(nserviceBusConnectionString);
        }
    }
}