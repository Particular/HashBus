namespace HashBus.Projector.TopTweeters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ColoredConsole;
    using HashBus.ReadModel;
    using LiteGuard;
    using NServiceBus;

    public class TopTweetersProjection : IHandleMessages<Twitter.Analyzer.Events.TweetAnalyzed>
    {
        private readonly IRepository<string, IEnumerable<Tweet>> tweets;

        public TopTweetersProjection(IRepository<string, IEnumerable<Tweet>> tweets)
        {
            Guard.AgainstNullArgument(nameof(tweets), tweets);

            this.tweets = tweets;
        }

        public void Handle(Twitter.Analyzer.Events.TweetAnalyzed message)
        {
            if (message.Tweet.RetweetedTweet != null)
            {
                return;
            }

            var trackTweets = this.tweets.GetAsync(message.Tweet.Track).GetAwaiter().GetResult().ToList();
            if (trackTweets.Any(tweet => tweet.TweetId == message.Tweet.Id))
            {
                return;
            }

            trackTweets.Add(new Tweet
            {
                TweetedAt = message.Tweet.CreatedAt,
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
                " tweet to ".Gray(),
                $" {message.Tweet.Track} ".DarkCyan().On(ConsoleColor.White));
        }
    }
}
