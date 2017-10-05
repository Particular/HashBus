namespace HashBus.Twitter.Monitor
{
    using System;
    using System.Configuration;
    using System.Threading.Tasks;

    class Program
    {
        static Task Main()
        {
            var track = ConfigurationManager.AppSettings["Track"];
            var nserviceBusConnectionString = ConfigurationManager.AppSettings["NServiceBusConnectionString"];
            var endpointName = ConfigurationManager.AppSettings["EndpointName"];
            var analyzerAddress = ConfigurationManager.AppSettings["AnalyzerAddress"];

            var consumerKeyName = "HASHBUS_TWITTER_CONSUMER_KEY";
            var consumerSecretKeyName = "HASHBUS_TWITTER_CONSUMER_SECRET";
            var accessTokenSecretKeyName = "HASHBUS_TWITTER_ACCESS_TOKEN_SECRET";
            var accessTokenKeyName = "HASHBUS_TWITTER_ACCESS_TOKEN";

            var consumerKey = Environment.GetEnvironmentVariable(consumerKeyName);
            if (consumerKey == null)
            {
                throw new Exception($"{consumerKeyName} enviroment variable is not set.");
            }

            var consumerSecret = Environment.GetEnvironmentVariable(consumerSecretKeyName);
            if (consumerSecret == null)
            {
                throw new Exception($"{consumerSecretKeyName} enviroment variable is not set.");
            }

            var accessToken = Environment.GetEnvironmentVariable(accessTokenKeyName);
            if (accessToken == null)
            {
                throw new Exception($"{accessTokenKeyName} enviroment variable is not set.");
            }

            var accessTokenSecret = Environment.GetEnvironmentVariable(accessTokenSecretKeyName);
            if (accessTokenSecret == null)
            {
                throw new Exception($"{accessTokenSecretKeyName} enviroment variable is not set.");
            }

            Console.Title = typeof(Program).Assembly.GetName().Name;

            return App.Run(
                nserviceBusConnectionString,
                track,
                consumerKey,
                consumerSecret,
                accessToken,
                accessTokenSecret,
                endpointName,
                analyzerAddress);
        }
    }
}
