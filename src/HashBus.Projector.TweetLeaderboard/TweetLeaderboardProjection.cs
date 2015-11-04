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
    public class TweetLeaderboardProjection : IHandleMessages<TweetWithHashtag>
    {
        private readonly IRepository<string, IEnumerable<Tweet>> tweets;

        public TweetLeaderboardProjection(IRepository<string, IEnumerable<Tweet>> tweets)
        {
            Guard.AgainstNullArgument(nameof(tweets), tweets);

            this.tweets = tweets;
        }

        public void Handle(TweetWithHashtag message)
        {
            var hashtagTweets = this.tweets.GetAsync(message.Hashtag).GetAwaiter().GetResult().ToList();
            if (hashtagTweets.Any(mention => mention.TweetId == message.TweetId))
            {
                return;
            }

            hashtagTweets.Add(new Tweet
            {
                TweetId = message.TweetId,
                UserId = message.TweetCreatedById,
                UserIdStr = message.TweetCreatedByIdStr,
                UserName = message.TweetCreatedByName,
                UserScreenName = message.TweetCreatedByScreenName,
            });

            this.tweets.SaveAsync(message.Hashtag, hashtagTweets).GetAwaiter().GetResult();

            ColorConsole.WriteLine(
                "Added ".Gray(),
                $"@{message.TweetCreatedByScreenName}".Cyan(),
                " tweet to ".Gray(),
                $"#{message.Hashtag}".DarkCyan().On(ConsoleColor.White),
                " leaderboard".Gray(),
                $" · {message.TweetCreatedAt.ToLocalTime()}".DarkGray());
        }
    }
}
