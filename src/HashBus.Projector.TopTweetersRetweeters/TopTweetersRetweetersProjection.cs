namespace HashBus.Projector.TopTweetersRetweeters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ColoredConsole;
    using HashBus.ReadModel;
    using LiteGuard;
    using NServiceBus;

    public class TopTweetersRetweetersProjection : IHandleMessages<Twitter.Analyzer.Events.TweetAnalyzed>
    {
        private readonly IRepository<string, IEnumerable<TweetRetweet>> tweets;

        public TopTweetersRetweetersProjection(IRepository<string, IEnumerable<TweetRetweet>> tweets)
        {
            Guard.AgainstNullArgument(nameof(tweets), tweets);

            this.tweets = tweets;
        }

        public async Task Handle(Twitter.Analyzer.Events.TweetAnalyzed message, IMessageHandlerContext context)
        {
            var trackTweets = (await this.tweets.GetAsync(message.Tweet.Track)
                .ConfigureAwait(false)).ToList();

            if (trackTweets.Any(tweet => tweet.TweetId == message.Tweet.Id))
            {
                return;
            }

            trackTweets.Add(new TweetRetweet
            {
                TweetedRetweetedAt = message.Tweet.CreatedAt,
                TweetId = message.Tweet.Id,
                UserId = message.Tweet.CreatedById,
                UserIdStr = message.Tweet.CreatedByIdStr,
                UserName = message.Tweet.CreatedByName,
                UserScreenName = message.Tweet.CreatedByScreenName,
            });

            await this.tweets.SaveAsync(message.Tweet.Track, trackTweets)
                .ConfigureAwait(false);

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
