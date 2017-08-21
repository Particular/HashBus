﻿namespace HashBus.Twitter.Analyzer
{
    using System;
    using System.Linq;
    using HashBus.Twitter.Analyzer.Commands;
    using HashBus.Twitter.Analyzer.Events;
    using LiteGuard;
    using NServiceBus;

    public class AnalyzeTweetHandler : IHandleMessages<AnalyzeTweet>
    {
        private ISendOnlyBus bus;

        public AnalyzeTweetHandler(ISendOnlyBus bus)
        {
            Guard.AgainstNullArgument("bus", bus);

            this.bus = bus;
        }

        public void Handle(AnalyzeTweet message)
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
            bus.Publish(new TweetAnalyzed { Tweet = TweetMapper.Map(message.Tweet) });
        }
    }
}
