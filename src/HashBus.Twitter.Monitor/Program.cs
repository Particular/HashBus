namespace HashBus.Twitter.Monitor
{
    using System;
    using System.Configuration;

    class Program
    {
        static void Main()
        {
            var hashtag = ConfigurationManager.AppSettings["hashtag"];

            var consumerKey = Environment.GetEnvironmentVariable("HASHBUS_TWITTER_CONSUMER_KEY");
            if (consumerKey == null)
            {
                throw new ArgumentException("Please make sure you have the 'HASHBUS_TWITTER_CONSUMER_KEY' set in your enviroment variables");
            }

            var consumerSecret = Environment.GetEnvironmentVariable("HASHBUS_TWITTER_CONSUMER_SECRET");
            if (consumerSecret == null)
            {
                throw new ArgumentException("Please make sure you have the 'HASHBUS_TWITTER_CONSUMER_SECRET' set in your enviroment variables");
            }

            var accessToken = Environment.GetEnvironmentVariable("HASHBUS_TWITTER_ACCESS_TOKEN");
            if (accessToken == null)
            {
                throw new ArgumentException("Please make sure you have the 'HASHBUS_TWITTER_ACCESS_TOKEN' set in your enviroment variables");
            }


            var accessTokenSecret = Environment.GetEnvironmentVariable("HASHBUS_TWITTER_ACCESS_TOKEN_SECRET");
            if (accessTokenSecret == null)
            {
                throw new ArgumentException("Please make sure you have the 'HASHBUS_TWITTER_ACCESS_TOKEN_SECRET' set in your enviroment variables");
            }

            App.RunAsync(hashtag, consumerKey, consumerSecret, accessToken, accessTokenSecret).GetAwaiter().GetResult();
        }
    }
}
