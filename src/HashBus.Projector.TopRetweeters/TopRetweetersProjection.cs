namespace HashBus.Projector.TopRetweeters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ColoredConsole;
    using HashBus.ReadModel;
    using LiteGuard;
    using NServiceBus;

    public class TopRetweetersProjection : IHandleMessages<Twitter.Analyzer.Events.TweetAnalyzed>
    {
        private readonly IRepository<string, IEnumerable<Retweet>> tweets;

        public TopRetweetersProjection(IRepository<string, IEnumerable<Retweet>> tweets)
        {
            Guard.AgainstNullArgument(nameof(tweets), tweets);

            this.tweets = tweets;
        }

        public void Handle(Twitter.Analyzer.Events.TweetAnalyzed message)
        {
            if (message.Tweet.RetweetedTweet == null)
            {
                return;
            }

            var trackTweets = this.tweets.GetAsync(message.Tweet.Track).GetAwaiter().GetResult().ToList();
            if (trackTweets.Any(tweet => tweet.TweetId == message.Tweet.Id))
            {
                return;
            }

            trackTweets.Add(new Retweet
            {
                RetweetedAt = message.Tweet.CreatedAt,
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
                " retweet to ".Gray(),
                $" {message.Tweet.Track} ".DarkCyan().On(ConsoleColor.White));
        }
    }
}
