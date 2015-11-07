namespace HashBus.Application
{
    using System;
    using System.Linq;
    using HashBus.Application.Events;
    using HashBus.Twitter.Events;
    using LiteGuard;
    using NServiceBus;
    using ColoredConsole;

    public class TweetReceivedHandler : IHandleMessages<TweetReceived>
    {
        private ISendOnlyBus bus;

        public TweetReceivedHandler(ISendOnlyBus bus)
        {
            Guard.AgainstNullArgument("bus", bus);

            this.bus = bus;
        }

        public void Handle(TweetReceived message)
        {
            if (!message.TweetIsRetweet)
            {
                var @event = new UserTweeted
                {
                    Track = message.Track,
                    TweetCreatedAt = message.TweetCreatedAt,
                    TweetCreatedById = message.TweetCreatedById,
                    TweetCreatedByIdStr = message.TweetCreatedByIdStr,
                    TweetCreatedByName = message.TweetCreatedByName,
                    TweetCreatedByScreenName = message.TweetCreatedByScreenName,
                    TweetId = message.TweetId,
                    TweetText = message.TweetText,
                };

                ColorConsole.WriteLine(
                    $"{message.TweetCreatedAt.ToLocalTime()}".DarkGray(),
                    " ",
                    $" {@event.Track} ".DarkCyan().On(ConsoleColor.White),
                    " ",
                    "tweet by".Gray(),
                    " ",
                    $"@{@event.TweetCreatedByScreenName}".Cyan());

                bus.Publish(@event);
            }

            foreach (var @event in message.TweetUserMentions
                .Where(userMention =>
                    message.TweetCreatedById != userMention.Id &&
                    message.TweetCreatedByIdStr != userMention.IdStr &&
                    message.TweetCreatedByScreenName != userMention.ScreenName &&
                    message.RetweetedTweetCreatedById != userMention.Id &&
                    message.RetweetedTweetCreatedByIdStr != userMention.IdStr &&
                    message.RetweetedTweetCreatedByScreenName != userMention.ScreenName &&
                    message.TweetText.Substring(0, userMention.Indices[0]).Trim().ToUpperInvariant() != "RT")
                .Select(userMention => new UserMentioned
                {
                    Track = message.Track,
                    TweetId = message.TweetId,
                    TweetCreatedAt = message.TweetCreatedAt,
                    TweetCreatedById = message.TweetCreatedById,
                    TweetCreatedByIdStr = message.TweetCreatedByIdStr,
                    TweetCreatedByName = message.TweetCreatedByName,
                    TweetCreatedByScreenName = message.TweetCreatedByScreenName,
                    TweetIsRetweet = message.TweetIsRetweet,
                    TweetText = message.TweetText,
                    UserMentionId = userMention.Id,
                    UserMentionIdStr = userMention.IdStr,
                    UserMentionName = userMention.Name,
                    UserMentionScreenName = userMention.ScreenName,
                }))
            {
                ColorConsole.WriteLine(
                    $"{message.TweetCreatedAt.ToLocalTime()}".DarkGray(),
                    " ",
                    $" {@event.Track} ".DarkCyan().On(ConsoleColor.White),
                    " ",
                    (@event.TweetIsRetweet ? "retweet" : "tweet").Gray(),
                    " ",
                    "mentioning".Gray(),
                    " ",
                    $"@{@event.UserMentionScreenName}".Cyan());

                bus.Publish(@event);
            }
        }
    }
}
