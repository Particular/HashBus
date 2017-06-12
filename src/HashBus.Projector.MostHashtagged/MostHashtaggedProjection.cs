namespace HashBus.Projector.MostHashtagged
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ColoredConsole;
    using HashBus.ReadModel;
    using LiteGuard;
    using NServiceBus;

    public class MostHashtaggedProjection : IHandleMessages<Twitter.Analyzer.Events.TweetAnalyzed>
    {
        private readonly IRepository<string, IEnumerable<Hashtag>> hashtags;

        public MostHashtaggedProjection(IRepository<string, IEnumerable<Hashtag>> hashtags)
        {
            Guard.AgainstNullArgument(nameof(hashtags), hashtags);

            this.hashtags = hashtags;
        }

        public async Task Handle(Twitter.Analyzer.Events.TweetAnalyzed message, IMessageHandlerContext context)
        {
            if (!message.Tweet.Hashtags.Any())
            {
                return;
            }

            var trackHashtags = (await this.hashtags.GetAsync(message.Tweet.Track)
                .ConfigureAwait(false)).ToList();

            if (trackHashtags.Any(hashtag => hashtag.TweetId == message.Tweet.Id))
            {
                return;
            }

            var newHashtags = message.Tweet.Hashtags.Select(hashtag =>
                    new Hashtag
                    {
                        HashtaggedAt = message.Tweet.CreatedAt,
                        TweetId = message.Tweet.Id,
                        Text = hashtag.Text,
                    })
                .ToList();

            trackHashtags.AddRange(newHashtags);

            await this.hashtags.SaveAsync(message.Tweet.Track, trackHashtags)
                .ConfigureAwait(false);

            foreach (var hashtag in newHashtags)
            {
                ColorConsole.WriteLine(
                    $"{message.Tweet.CreatedAt.ToLocalTime()}".DarkGray(),
                    " ",
                    "Added ".Gray(),
                    $"#{hashtag.Text}".Cyan(),
                    " usage to ".Gray(),
                    $" {message.Tweet.Track} ".DarkCyan().On(ConsoleColor.White));
            }
        }
    }
}
