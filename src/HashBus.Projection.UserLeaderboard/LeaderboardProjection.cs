using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColoredConsole;
using HashBus.Application.Events;
using LiteGuard;
using NServiceBus;

namespace HashBus.Projection.UserLeaderboard
{
    public class LeaderboardProjection : IHandleMessages<UserMentionedWithHashtag>
    {
        private readonly IRepository<string, IEnumerable<Mention>> mentions;

        public LeaderboardProjection(IRepository<string, IEnumerable<Mention>> mentions)
        {
            Guard.AgainstNullArgument(nameof(mentions), mentions);

            this.mentions = mentions;
        }

        public async Task Handle(UserMentionedWithHashtag message)
        {
            var hashtagMentions = (await this.mentions.GetAsync(message.Hashtag)).ToList();
            if (!message.UserMentionId.HasValue ||
                hashtagMentions.Any(mention => mention.TweetId == message.TweetId && mention.UserMentionId == message.UserMentionId))
            {
                return;
            }

            hashtagMentions.Add(new Mention
            {
                TweetId = message.TweetId,
                UserMentionId = message.UserMentionId,
                UserMentionIdStr = message.UserMentionIdStr,
                UserMentionName = message.UserMentionName,
                UserMentionScreenName = message.UserMentionScreenName,
            });

            await this.mentions.SaveAsync(message.Hashtag, hashtagMentions);

            ColorConsole.WriteLine(
                "Added ".Gray(),
                $"@{message.UserMentionScreenName}".Cyan(),
                " mention to ".Gray(),
                $"#{message.Hashtag}".DarkCyan().On(ConsoleColor.White),
                " leaderboard".Gray(),
                $" · {message.TweetCreatedAt}".DarkGray());
        }
    }
}
