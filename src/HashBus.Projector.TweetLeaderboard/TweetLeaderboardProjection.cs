using System;
using System.Collections.Generic;
using System.Linq;
using ColoredConsole;
using HashBus.Application.Events;
using HashBus.ReadModel;
using LiteGuard;
using NServiceBus;

namespace HashBus.Projector.TweetLeaderboard
{
    public class TweetLeaderboardProjection : IHandleMessages<UserTweeted>
    {
        private readonly IRepository<string, IEnumerable<Tweet>> tweets;

        public TweetLeaderboardProjection(IRepository<string, IEnumerable<Tweet>> tweets)
        {
            Guard.AgainstNullArgument(nameof(tweets), tweets);

            this.tweets = tweets;
        }

        public void Handle(UserTweeted message)
        {
            var trackTweets = this.tweets.GetAsync(message.Track).GetAwaiter().GetResult().ToList();
            if (trackTweets.Any(mention => mention.TweetId == message.TweetId))
            {
                return;
            }

            trackTweets.Add(new Tweet
            {
                TweetId = message.TweetId,
                UserId = message.TweetCreatedById,
                UserIdStr = message.TweetCreatedByIdStr,
                UserName = message.TweetCreatedByName,
                UserScreenName = message.TweetCreatedByScreenName,
            });

            this.tweets.SaveAsync(message.Track, trackTweets).GetAwaiter().GetResult();

            ColorConsole.WriteLine(
                "Added ".Gray(),
                $"@{message.TweetCreatedByScreenName}".Cyan(),
                " tweet to ".Gray(),
                $" {message.Track} ".DarkCyan().On(ConsoleColor.White),
                $" · {message.TweetCreatedAt.ToLocalTime()}".DarkGray());
        }
    }
}
