namespace HashBus.Twitter.Monitor
{
    using System;
    using System.Configuration;

    class Program
    {
        static void Main()
        {
            var hashtag = ConfigurationManager.AppSettings["hashtag"];

            const string consumerKeyName = "HASHBUS_TWITTER_CONSUMER_KEY";
            const string consumerSecretKeyName = "HASHBUS_TWITTER_CONSUMER_SECRET";
            const string accessTokenSecretKeyName = "HASHBUS_TWITTER_ACCESS_TOKEN_SECRET";
            const string accessTokenKeyName = "HASHBUS_TWITTER_ACCESS_TOKEN";

            var consumerKey = Environment.GetEnvironmentVariable(consumerKeyName);
            if (consumerKey == null)
            {
                throw new ArgumentException("Please make sure you have the {0} set in your enviroment variables", consumerKeyName);
            }

            var consumerSecret = Environment.GetEnvironmentVariable(consumerSecretKeyName);
            if (consumerSecret == null)
            {
                throw new ArgumentException("Please make sure you have the {0} set in your enviroment variables", consumerSecretKeyName);
            }

            var accessToken = Environment.GetEnvironmentVariable(accessTokenKeyName);
            if (accessToken == null)
            {
                throw new ArgumentException("Please make sure you have the {0} set in your enviroment variables", accessTokenKeyName);
            }

            var accessTokenSecret = Environment.GetEnvironmentVariable(accessTokenSecretKeyName);
            if (accessTokenSecret == null)
            {
                throw new ArgumentException("Please make sure you have the {0} set in your enviroment variables", accessTokenSecretKeyName);
            }

            App.RunAsync(hashtag, consumerKey, consumerSecret, accessToken, accessTokenSecret).GetAwaiter().GetResult();
        }
    }
}
