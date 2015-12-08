namespace HashBus.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using HashBus.ReadModel;
    using Nancy;

    public class MostHashtaggedModule : NancyModule
    {
        public MostHashtaggedModule(IRepository<string, IEnumerable<Hashtag>> hashtags)
        {
            this.Get["/most-hashtagged/{track}", true] = async (parameters, __) =>
            {
                // see https://github.com/NancyFx/Nancy/issues/1154
                var track = ((string)parameters.track).Replace("해시", "#");
                var trackHashtags = (await hashtags.GetAsync(track)).ToList();
                var entries = trackHashtags
                    .GroupBy(tweet => tweet.Text, StringComparer.InvariantCultureIgnoreCase)
                    .Select(g => new HashtagEntry
                    {
                        Text = g.Key,
                        Count = g.Count(),
                    })
                    .OrderByDescending(entry => entry.Count)
                    .Select((entry, index) =>
                    {
                        entry.Position = index + 1;
                        return entry;
                    })
                    .Take(10)
                    .ToList();

                return new Leaderboard<HashtagEntry>
                {
                    Entries = entries,
                    Count = trackHashtags.Count,
                    Since = trackHashtags.Min(hashtag => hashtag.HashtaggedAt),
                    LastActivityDateTime = trackHashtags.Max(hashtag => hashtag.HashtaggedAt),
                };
            };
        }
    }
}
