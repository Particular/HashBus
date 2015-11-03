using System.Collections.Generic;
using System.Linq;
using HashBus.ReadModel;
using Nancy;

namespace HashBus.WebApi
{
    public class MentionLeaderboardModule : NancyModule
    {
        public MentionLeaderboardModule(IRepository<string, IEnumerable<Mention>> mentions)
        {
            this.Get["/mention-leaderboards/{hashtag}", true] = async (parameters, __) =>
            {
                var hashtagMentions = (await mentions.GetAsync((string)parameters.hashtag)).ToList();
                var entries = hashtagMentions
                    .GroupBy(mention => mention.UserMentionId)
                    .Select(g => new MentionLeaderboard.Entry
                    {
                        UserMentionId = g.Key,
                        UserMentionIdStr = g.First().UserMentionIdStr,
                        UserMentionName = g.First().UserMentionName,
                        UserMentionScreenName = g.First().UserMentionScreenName,
                        Count = g.Count(),
                    })
                    .OrderByDescending(entry => entry.Count)
                    .Take(10)
                    .ToList();

                return new MentionLeaderboard
                {
                    Entries = entries,
                    MentionsCount = hashtagMentions.Count,
                };
            };
        }
    }
}
