namespace HashBus.Twitter.Analyzer
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using HashBus.Twitter.Analyzer.Commands;
    using HashBus.Twitter.Analyzer.Events;
    using NServiceBus;

    public class AnalyzeTweetHandler : IHandleMessages<AnalyzeTweet>
    {
        public Task Handle(AnalyzeTweet message, IMessageHandlerContext context)
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

            return context.Publish(new TweetAnalyzed { Tweet = TweetMapper.Map(message.Tweet) });
        }
    }
}
