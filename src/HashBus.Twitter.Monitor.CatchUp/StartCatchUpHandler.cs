namespace HashBus.Twitter.Monitor.CatchUp
{
    using System;
    using System.Linq;
    using Commands;
    using Events;
    using NHibernate.Util;
    using NServiceBus;
    using Tweetinvi;
    using Tweetinvi.Core.Credentials;
    using Tweetinvi.Core.Parameters;

    public class StartCatchUpHandler : IHandleMessages<StartCatchUp>
    {
        public IBus Bus { get; set; }

        public void Handle(StartCatchUp message)
        {
            // TODO: call twitter API and get the latest tweets since the provided TweetId
            Console.WriteLine("==================             ====================");

            Console.WriteLine(" Starting to catchup form tweetId: {0}", message.TweetId);

            PublishCatchUpTweets(message);
        }

        private void PublishCatchUpTweets(StartCatchUp message)
        {
            TwitterCredentialsHelper.SetupTweeterCredentials();

            var searchParameters = new TweetSearchParameters(message.Track)
            {
                SinceId = message.TweetId
            };

            Search.SearchTweets(searchParameters).ForEach(e =>
            {
                var tweetReceived = new TweetReceived
                {
                    EndpointName = message.EndpointName,
                    SessionId = message.SessionId,
                    Track = message.Track,
                    TweetId = e.Id,
                    TweetCreatedAt = e.CreatedAt,
                    TweetCreatedById = e.CreatedBy.Id,
                    TweetCreatedByIdStr = e.CreatedBy.IdStr,
                    TweetCreatedByName = e.CreatedBy.Name,
                    TweetCreatedByScreenName = e.CreatedBy.ScreenName,
                    TweetIsRetweet = e.IsRetweet,
                    TweetText = e.Text,
                    TweetUserMentions = e.UserMentions
                        .Select(userMention => new UserMention
                        {
                            Id = userMention.Id,
                            IdStr = userMention.IdStr,
                            Indices = userMention.Indices,
                            Name = userMention.Name,
                            ScreenName = userMention.ScreenName,
                        })
                        .ToArray(),
                    RetweetedTweetId = e.IsRetweet ? e.RetweetedTweet.Id : default(long),
                    RetweetedTweetCreatedAt = e.IsRetweet ? e.RetweetedTweet.CreatedAt : default(DateTime),
                    RetweetedTweetCreatedById = e.IsRetweet ? e.RetweetedTweet.CreatedBy.Id : default(long),
                    RetweetedTweetCreatedByIdStr = e.IsRetweet ? e.RetweetedTweet.CreatedBy.IdStr : default(string),
                    RetweetedTweetCreatedByName = e.IsRetweet ? e.RetweetedTweet.CreatedBy.Name : default(string),
                    RetweetedTweetCreatedByScreenName = e.IsRetweet ? e.RetweetedTweet.CreatedBy.ScreenName : default(string),
                    TweetHashtags = e.Hashtags
                        .Select(ht => new Hashtag
                        {
                            Text = ht.Text,
                            Indices = ht.Indices,
                        }).ToArray()
                };

                Writer.Write(tweetReceived);
                Bus.Publish(tweetReceived);
            });
        }
    }

    internal class TwitterCredentialsHelper
    {
        internal static void SetupTweeterCredentials()
        {
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

            var twitterCredentials = new TwitterCredentials(consumerKey, consumerSecret, accessToken, accessTokenSecret);

            Auth.SetCredentials(twitterCredentials);
        }
    }
}
