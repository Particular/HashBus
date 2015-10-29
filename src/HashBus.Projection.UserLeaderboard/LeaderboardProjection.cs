using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ColoredConsole;
using HashBus.Application.Events;
using LiteGuard;
using NServiceBus;

namespace HashBus.Projection.UserLeaderboard
{
    public class LeaderboardProjection : IHandleMessages<UserMentionedWithHashtag>
    {
        private static readonly Dictionary<int, string> movementTokens =
            new Dictionary<int, string>
            {
                { -1, "↑" },
                { 0, "→" },
                { 1, "↓" },
            };

        private static readonly Dictionary<int, ConsoleColor> movementColors =
            new Dictionary<int, ConsoleColor>
            {
                { -1, ConsoleColor.Green },
                { 0, ConsoleColor.White },
                { 1, ConsoleColor.Red },
            };

        private readonly IRepository<string, IEnumerable<Mention>> mentions;

        public LeaderboardProjection(IRepository<string, IEnumerable<Mention>> mentions)
        {
            Guard.AgainstNullArgument(nameof(mentions), mentions);

            this.mentions = mentions;
        }

        public async Task Handle(UserMentionedWithHashtag message)
        {
            var hashtagMentions = (await this.mentions.GetAsync(message.Hashtag)).ToList();
            if (!message.UserMentionId.HasValue ||
                hashtagMentions.Any(mention => mention.TweetId == message.TweetId && mention.UserMentionId == message.UserMentionId))
            {
                return;
            }

            var previousLeaderboard = GetLeaderBoard(hashtagMentions)
                .Select((entry, index) => new { Position = index + 1, Entry = entry })
                .ToList();

            hashtagMentions.Add(new Mention
            {
                TweetId = message.TweetId,
                UserMentionId = message.UserMentionId,
                UserMentionIdStr = message.UserMentionIdStr,
                UserMentionName = message.UserMentionName,
                UserMentionScreenName = message.UserMentionScreenName,
            });

            await this.mentions.SaveAsync(message.Hashtag, hashtagMentions);

            var position = 0;
            var lines = new List<ColorToken[]>();
            var haveMovement = false;
            foreach (var entry in GetLeaderBoard(hashtagMentions))
            {
                ++position;
                var previousEntry = previousLeaderboard.FirstOrDefault(e => e.Entry.UserMentionId == entry.UserMentionId);
                var movement = previousEntry == null
                    ? 0
                    : Math.Sign(position - previousEntry.Position);

                if (movement != 0)
                {
                    haveMovement = true;
                }

                lines.Add(new[]
                {
                    movementTokens[movement].Color(movementColors[movement]),
                    $" {position.ToString().PadLeft(2)}".Gray(),
                    $" {entry.UserMentionName}".Color(movementColors[movement]),
                    $" @{entry.UserMentionScreenName}".Cyan(),
                    $" {entry.Count:N0}".Yellow(),
                });
            }

            if (!haveMovement)
            {
                return;
            }

            ColorConsole.WriteLine();
            ColorConsole.WriteLine(
               $"#{message.Hashtag}".DarkCyan().On(ConsoleColor.White),
               $" · {message.TweetCreatedAt}".DarkGray());

            foreach(var line in lines)
            {
                ColorConsole.WriteLine(line);
            }

            ColorConsole.WriteLine("Total tweets: ".Gray(), $"{hashtagMentions.Count:N0}".Yellow());
        }

        private static IEnumerable<Entry> GetLeaderBoard(List<Mention> mentions)
        {
            return mentions
                .GroupBy(mention => mention.UserMentionId)
                .Select(g => new Entry
                {
                    UserMentionId = g.Key,
                    UserMentionIdStr = g.First().UserMentionIdStr,
                    UserMentionName = g.First().UserMentionName,
                    UserMentionScreenName = g.First().UserMentionScreenName,
                    Count = g.Count(),
                })
                .OrderByDescending(entry => entry.Count)
                .Take(10);
        }

        public class Mention
        {
            public long TweetId { get; set; }

            public long? UserMentionId { get; set; }

            public string UserMentionIdStr { get; set; }

            public string UserMentionName { get; set; }

            public string UserMentionScreenName { get; set; }
        }
        public class Entry
        {
            public long? UserMentionId { get; set; }

            public string UserMentionIdStr { get; set; }

            public string UserMentionName { get; set; }

            public string UserMentionScreenName { get; set; }

            public int Count { get; set; }
        }
    }
}
