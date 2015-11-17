namespace HashBus.Projector.MostHashtagged
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ColoredConsole;
    using HashBus.ReadModel;
    using LiteGuard;
    using NServiceBus;

    public class MostHashtaggedProjection : IHandleMessages<Application.Events.TweetAnalyzed>
    {
        private readonly IRepository<string, IEnumerable<Hashtag>> hashtags;

        public MostHashtaggedProjection(IRepository<string, IEnumerable<Hashtag>> hashtags)
        {
            Guard.AgainstNullArgument(nameof(hashtags), hashtags);

            this.hashtags = hashtags;
        }

        public void Handle(Application.Events.TweetAnalyzed message)
        {
            if (!message.Tweet.Hashtags.Any())
            {
                return;
            }

            var trackHashtags = this.hashtags.GetAsync(message.Tweet.Track).GetAwaiter().GetResult().ToList();
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

            this.hashtags.SaveAsync(message.Tweet.Track, trackHashtags).GetAwaiter().GetResult();

            foreach (var hashtag in newHashtags)
            {
                ColorConsole.WriteLine(
                    $"{message.Tweet.CreatedAt.ToLocalTime()}".DarkGray(),
                    " ",
                    "Added ".Gray(),
                    $"@{hashtag.Text}".Cyan(),
                    " usage to ".Gray(),
                    $" {message.Tweet.Track} ".DarkCyan().On(ConsoleColor.White));
            }
        }
    }
}
