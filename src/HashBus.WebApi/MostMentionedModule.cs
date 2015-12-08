namespace HashBus.WebApi
{
    using System.Collections.Generic;
    using System.Linq;
    using HashBus.ReadModel;
    using Nancy;

    public class MostMentionedModule : NancyModule
    {
        public MostMentionedModule(IRepository<string, IEnumerable<Mention>> mentions)
        {
            this.Get["/most-mentioned/{track}", true] = async (parameters, __) =>
            {
                // see https://github.com/NancyFx/Nancy/issues/1154
                var track = ((string)parameters.track).Replace("해시", "#");
                var trackMentions = (await mentions.GetAsync(track)).ToList();
                var entries = trackMentions
                    .GroupBy(mention => mention.UserMentionId)
                    .Select(g => new UserEntry
                    {
                        Id = g.Key,
                        IdStr = g.First().UserMentionIdStr,
                        Name = g.First().UserMentionName,
                        ScreenName = g.First().UserMentionScreenName,
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

                return new Leaderboard<UserEntry>
                {
                    Entries = entries,
                    Count = trackMentions.Count,
                    Since = trackMentions.Min(mention => mention.MentionedAt),
                    LastActivityDateTime = trackMentions.Max(mention => mention.MentionedAt),
                };
            };
        }
    }
}
