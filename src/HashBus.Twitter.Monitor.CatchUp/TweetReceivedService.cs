namespace HashBus.Twitter.Monitor.CatchUp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HashBus.Twitter.Events;
    using Tweetinvi;
    using Tweetinvi.Core.Credentials;
    using Tweetinvi.Core.Parameters;

    class TweetReceivedService : ITweetReceivedService
    {
        public TweetReceivedService(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            Auth.SetCredentials(new TwitterCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret));
        }

        public IEnumerable<TweetReceived> Get(string track, long sinceTweetId, string endpointName, Guid sessionId)
        {
            return Search
                .SearchTweets(new TweetSearchParameters(track) { SinceId = sinceTweetId, })
                .Select(tweet => TweetMapper.Map(tweet, endpointName, sessionId, track));
        }
    }
}
