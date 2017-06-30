namespace HashBus.Twitter.BackFill
{
    using System;
    using System.Configuration;

    class Program
    {
        static void Main()
        {
            var nserviceBusConnectionString = ConfigurationManager.AppSettings["NServiceBusConnectionString"];
            var endpointName = ConfigurationManager.AppSettings["EndpointName"];
            var track = ConfigurationManager.AppSettings["Track"];
            var tweetId = long.Parse(ConfigurationManager.AppSettings["TweetId"]);

            Console.Title = typeof(Program).Assembly.GetName().Name;

            App.Run(nserviceBusConnectionString, endpointName, track, tweetId);
        }
    }
}
