using System;
using System.Collections.Generic;
using System.Linq;
using ColoredConsole;
using HashBus.Application.Events;
using HashBus.ReadModel;
using LiteGuard;
using NServiceBus;

namespace HashBus.Projector.MentionLeaderboard
{
    public class MentionLeaderboardProjection : IHandleMessages<UserMentionedWithHashtag>
    {
        private readonly IRepository<string, IEnumerable<Mention>> mentions;

        public MentionLeaderboardProjection(IRepository<string, IEnumerable<Mention>> mentions)
        {
            Guard.AgainstNullArgument(nameof(mentions), mentions);

            this.mentions = mentions;
        }

        public void Handle(UserMentionedWithHashtag message)
        {
            var hashtagMentions = this.mentions.GetAsync(message.Hashtag).GetAwaiter().GetResult().ToList();
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

            this.mentions.SaveAsync(message.Hashtag, hashtagMentions).GetAwaiter().GetResult();

            ColorConsole.WriteLine(
                "Added ".Gray(),
                $"@{message.UserMentionScreenName}".Cyan(),
                " mention to ".Gray(),
                $"#{message.Hashtag}".DarkCyan().On(ConsoleColor.White),
                " leaderboard".Gray(),
                $" · {message.TweetCreatedAt.ToLocalTime()}".DarkGray());
        }
    }
}
