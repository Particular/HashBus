namespace HashBus.Projector.MostMentioned
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ColoredConsole;
    using HashBus.Twitter.Analyzer.Events;
    using HashBus.ReadModel;
    using LiteGuard;
    using NServiceBus;

    public class MostMentionedProjection : IHandleMessages<TweetAnalyzed>
    {
        private readonly IRepository<string, IEnumerable<Mention>> mentions;

        public MostMentionedProjection(IRepository<string, IEnumerable<Mention>> mentions)
        {
            Guard.AgainstNullArgument(nameof(mentions), mentions);

            this.mentions = mentions;
        }

        public async Task Handle(TweetAnalyzed message, IMessageHandlerContext context)
        {
            if (!message.Tweet.UserMentions.Any())
            {
                return;
            }

            var trackMentions = (await this.mentions.GetAsync(message.Tweet.Track)
                .ConfigureAwait(false)).ToList();

            if (trackMentions.Any(mention => mention.TweetId == message.Tweet.Id))
            {
                return;
            }

            var newMentions = message.Tweet.UserMentions.Select(userMention =>
                    new Mention
                    {
                        MentionedAt = message.Tweet.CreatedAt,
                        TweetId = message.Tweet.Id,
                        UserMentionId = userMention.Id,
                        UserMentionIdStr = userMention.IdStr,
                        UserMentionName = userMention.Name,
                        UserMentionScreenName = userMention.ScreenName,
                    })
                .ToList();

            trackMentions.AddRange(newMentions);

            await this.mentions.SaveAsync(message.Tweet.Track, trackMentions)
                .ConfigureAwait(false);

            foreach (var mention in newMentions)
            {
                ColorConsole.WriteLine(
                    $"{message.Tweet.CreatedAt.ToLocalTime()}".DarkGray(),
                    " ",
                    "Added ".Gray(),
                    $"@{mention.UserMentionScreenName}".Cyan(),
                    " mention to ".Gray(),
                    $" {message.Tweet.Track} ".DarkCyan().On(ConsoleColor.White));
            }
        }
    }
}
