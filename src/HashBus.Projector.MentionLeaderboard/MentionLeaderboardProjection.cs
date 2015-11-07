namespace HashBus.Projector.MentionLeaderboard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ColoredConsole;
    using HashBus.Application.Events;
    using HashBus.ReadModel;
    using LiteGuard;
    using NServiceBus;

    public class MentionLeaderboardProjection : IHandleMessages<UserMentioned>
    {
        private readonly IRepository<string, IEnumerable<Mention>> mentions;

        public MentionLeaderboardProjection(IRepository<string, IEnumerable<Mention>> mentions)
        {
            Guard.AgainstNullArgument(nameof(mentions), mentions);

            this.mentions = mentions;
        }

        public void Handle(UserMentioned message)
        {
            var trackMentions = this.mentions.GetAsync(message.Track).GetAwaiter().GetResult().ToList();
            if (!message.UserMentionId.HasValue ||
                trackMentions.Any(mention => mention.TweetId == message.TweetId && mention.UserMentionId == message.UserMentionId))
            {
                return;
            }

            trackMentions.Add(new Mention
            {
                TweetId = message.TweetId,
                UserMentionId = message.UserMentionId,
                UserMentionIdStr = message.UserMentionIdStr,
                UserMentionName = message.UserMentionName,
                UserMentionScreenName = message.UserMentionScreenName,
            });

            this.mentions.SaveAsync(message.Track, trackMentions).GetAwaiter().GetResult();

            ColorConsole.WriteLine(
                $"{message.TweetCreatedAt.ToLocalTime()}".DarkGray(),
                " ",
                "Added ".Gray(),
                $"@{message.UserMentionScreenName}".Cyan(),
                " mention to ".Gray(),
                $" {message.Track} ".DarkCyan().On(ConsoleColor.White));
        }
    }
}
