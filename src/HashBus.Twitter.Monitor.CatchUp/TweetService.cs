namespace HashBus.Twitter.Monitor.CatchUp
{
    using System.Collections.Generic;
    using Tweetinvi;
    using Tweetinvi.Core.Credentials;
    using Tweetinvi.Core.Interfaces;
    using Tweetinvi.Core.Parameters;

    class TweetService : ITweetService
    {
        private readonly int maximumNumberOfTweets;

        public TweetService(int maximumNumberOfTweets, string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            this.maximumNumberOfTweets = maximumNumberOfTweets;
            Auth.SetCredentials(new TwitterCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret));
        }

        public IEnumerable<ITweet> Get(string track, long sinceTweetId)
        {
            return Search
                .SearchTweets(new TweetSearchParameters(track) { SinceId = sinceTweetId, MaximumNumberOfResults = this.maximumNumberOfTweets });
        }
    }
}
