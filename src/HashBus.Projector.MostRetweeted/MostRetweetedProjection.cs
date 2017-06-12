namespace HashBus.Projector.MostRetweeted
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ColoredConsole;
    using HashBus.ReadModel;
    using LiteGuard;
    using NServiceBus;

    public class MostRetweetedProjection : IHandleMessages<Twitter.Analyzer.Events.TweetAnalyzed>
    {
        private readonly IRepository<string, IEnumerable<Retweetee>> retweetees;

        public MostRetweetedProjection(IRepository<string, IEnumerable<Retweetee>> retweetees)
        {
            Guard.AgainstNullArgument(nameof(retweetees), retweetees);

            this.retweetees = retweetees;
        }

        public async Task Handle(Twitter.Analyzer.Events.TweetAnalyzed message, IMessageHandlerContext context)
        {
            if (message.Tweet.RetweetedTweet == null)
            {
                return;
            }

            var trackRetweetees = (await this.retweetees.GetAsync(message.Tweet.Track)
                .ConfigureAwait(false)).ToList();

            if (trackRetweetees.Any(tweet => tweet.TweetId == message.Tweet.Id))
            {
                return;
            }

            var rewRetweetees = new List<Retweetee>();
            var retweet = message.Tweet.RetweetedTweet;
            while (retweet != null)
            {
                rewRetweetees.Add(new Retweetee
                {
                    RetweetedAt = message.Tweet.CreatedAt,
                    TweetId = retweet.Id,
                    UserId = retweet.CreatedById,
                    UserIdStr = retweet.CreatedByIdStr,
                    UserName = retweet.CreatedByName,
                    UserScreenName = retweet.CreatedByScreenName,
                });

                retweet = retweet.RetweetedTweet;
            }

            trackRetweetees.AddRange(rewRetweetees);
            await this.retweetees.SaveAsync(message.Tweet.Track, trackRetweetees)
                .ConfigureAwait(false);

            foreach (var retweetee in rewRetweetees)
            {
                ColorConsole.WriteLine(
                    $"{message.Tweet.CreatedAt.ToLocalTime()}".DarkGray(),
                    " ",
                    "Added retweet of ".Gray(),
                    $"@{retweetee.UserScreenName}".Cyan(),
                    " to ".Gray(),
                    $" {message.Tweet.Track} ".DarkCyan().On(ConsoleColor.White));
            }
        }
    }
}
