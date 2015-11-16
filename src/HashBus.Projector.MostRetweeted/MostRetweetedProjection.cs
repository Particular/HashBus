namespace HashBus.Projector.MostRetweeted
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ColoredConsole;
    using HashBus.ReadModel;
    using LiteGuard;
    using NServiceBus;

    public class MostRetweetedProjection : IHandleMessages<Application.Events.TweetAnalyzed>
    {
        private readonly IRepository<string, IEnumerable<Retweetee>> retweetees;

        public MostRetweetedProjection(IRepository<string, IEnumerable<Retweetee>> retweetees)
        {
            Guard.AgainstNullArgument(nameof(retweetees), retweetees);

            this.retweetees = retweetees;
        }

        public void Handle(Application.Events.TweetAnalyzed message)
        {
            if (message.Tweet.RetweetedTweet == null)
            {
                return;
            }

            var trackRetweetees = this.retweetees.GetAsync(message.Tweet.Track).GetAwaiter().GetResult().ToList();
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
                    TweetId = retweet.Id,
                    UserId = retweet.CreatedById,
                    UserIdStr = retweet.CreatedByIdStr,
                    UserName = retweet.CreatedByName,
                    UserScreenName = retweet.CreatedByScreenName,
                });

                retweet = retweet.RetweetedTweet;
            }

            trackRetweetees.AddRange(rewRetweetees);
            this.retweetees.SaveAsync(message.Tweet.Track, trackRetweetees).GetAwaiter().GetResult();

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
