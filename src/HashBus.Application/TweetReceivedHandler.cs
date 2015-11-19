namespace HashBus.Application
{
    using System;
    using System.Linq;
    using HashBus.Application.Events;
    using HashBus.Twitter.Events;
    using LiteGuard;
    using NServiceBus;

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
            message.Tweet.UserMentions = message.Tweet.UserMentions
                .Where(userMention =>
                    message.Tweet.CreatedById != userMention.Id &&
                    message.Tweet.CreatedByIdStr != userMention.IdStr &&
                    message.Tweet.CreatedByScreenName != userMention.ScreenName &&
                    (message.Tweet.RetweetedTweet == null ||
                        (message.Tweet.RetweetedTweet.CreatedById != userMention.Id &&
                        message.Tweet.RetweetedTweet.CreatedByIdStr != userMention.IdStr &&
                        message.Tweet.RetweetedTweet.CreatedByScreenName != userMention.ScreenName)) &&
                    message.Tweet.Text.Substring(0, userMention.Indices[0]).Trim().ToUpperInvariant() != "RT")
                .GroupBy(userMention => userMention.Id)
                .Select(group => group.First())
                .ToList();

            message.Tweet.Hashtags = message.Tweet.Hashtags
                .Where(hashtag => !string.Equals($"#{hashtag.Text}", message.Tweet.Track, StringComparison.OrdinalIgnoreCase))
                .GroupBy(hashtag => hashtag.Text.ToUpperInvariant())
                .Select(group => group.First())
                .ToList();

            Writer.Write(message.Tweet);
            bus.Publish(new TweetAnalyzed { Tweet = message.Tweet });
        }
    }
}
