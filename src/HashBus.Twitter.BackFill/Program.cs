namespace HashBus.Twitter.BackFill
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
            var track = ConfigurationManager.AppSettings["Track"];
            var tweetId = long.Parse(ConfigurationManager.AppSettings["TweetId"]);
            var catchUpAddress = ConfigurationManager.AppSettings["CatchUpAddress"];

            Console.Title = typeof(Program).Assembly.GetName().Name;

            return App.Run(nserviceBusConnectionString, endpointName, track, tweetId, catchUpAddress);
        }
    }
}
