namespace HashBus.Projector.TopTweetersRetweeters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ColoredConsole;
    using HashBus.ReadModel;
    using LiteGuard;
    using NServiceBus;

    public class TopTweetersRetweetersProjection : IHandleMessages<Application.Events.TweetAnalyzed>
    {
        private readonly IRepository<string, IEnumerable<TweetRetweet>> tweets;

        public TopTweetersRetweetersProjection(IRepository<string, IEnumerable<TweetRetweet>> tweets)
        {
            Guard.AgainstNullArgument(nameof(tweets), tweets);

            this.tweets = tweets;
        }

        public void Handle(Application.Events.TweetAnalyzed message)
        {
            var trackTweets = this.tweets.GetAsync(message.Tweet.Track).GetAwaiter().GetResult().ToList();
            if (trackTweets.Any(tweet => tweet.TweetId == message.Tweet.Id))
            {
                return;
            }

            trackTweets.Add(new TweetRetweet
            {
                TweetId = message.Tweet.Id,
                UserId = message.Tweet.CreatedById,
                UserIdStr = message.Tweet.CreatedByIdStr,
                UserName = message.Tweet.CreatedByName,
                UserScreenName = message.Tweet.CreatedByScreenName,
            });

            this.tweets.SaveAsync(message.Tweet.Track, trackTweets).GetAwaiter().GetResult();

            ColorConsole.WriteLine(
                $"{message.Tweet.CreatedAt.ToLocalTime()}".DarkGray(),
                " ",
                "Added ".Gray(),
                $"@{message.Tweet.CreatedByScreenName}".Cyan(),
                " tweet/retweet to ".Gray(),
                $" {message.Tweet.Track} ".DarkCyan().On(ConsoleColor.White));
        }
    }
}
