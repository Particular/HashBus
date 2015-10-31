using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ColoredConsole;
using HashBus.Projection;
using Humanizer;

namespace HashBus.View.MentionLeaderboard
{

    class Program
    {
        private static readonly Dictionary<int, string> movementTokens =
            new Dictionary<int, string>
            {
                { int.MaxValue, ">" },
                { -1, "^" },
                { 0, "=" },
                { 1, "v" },
            };

        private static readonly Dictionary<int, ConsoleColor> movementColors =
            new Dictionary<int, ConsoleColor>
            {
                { int.MaxValue, ConsoleColor.DarkYellow },
                { -1, ConsoleColor.DarkGreen },
                { 0, ConsoleColor.Gray },
                { 1, ConsoleColor.DarkRed },
            };

        private static readonly Dictionary<int, ConsoleColor> movementUserNameColors =
            new Dictionary<int, ConsoleColor>
            {
                { int.MaxValue, ConsoleColor.Yellow },
                { -1, ConsoleColor.Green },
                { 0, ConsoleColor.White },
                { 1, ConsoleColor.Red },
            };

        static void Main()
        {
            AsyncMain().GetAwaiter().GetResult();
        }

        static async Task AsyncMain()
        {
            var dataFolder = ConfigurationManager.AppSettings["DataFolder"];
            var hashtag = ConfigurationManager.AppSettings["hashTag"];
            var mentions = new FileListRepository<Mention>(Path.Combine(dataFolder, "MentionLeaderboardProjection.Mentions"));
            var refreshInterval = int.Parse(ConfigurationManager.AppSettings["refreshInterval"]);

            var start = DateTime.UtcNow;
            var initialCount = (int?)null;
            var previousLeaderboard = new List<Entry>();
            while (true)
            {
                var hashtagMentions = (await mentions.GetAsync(hashtag)).ToList();
                var currentLeaderboard = hashtagMentions
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
                    .Take(10).ToList();

                var position = 0;
                var lines = new List<ColorToken[]>();
                foreach (var currentEntry in currentLeaderboard)
                {
                    ++position;
                    var previousEntry = previousLeaderboard
                        .Select((entry, index) => new { Entry = entry, Position = index + 1 })
                        .FirstOrDefault(e => e.Entry.UserMentionId == currentEntry.UserMentionId);

                    var movement = previousEntry == null
                        ? int.MaxValue
                        : Math.Sign(position - previousEntry.Position);

                    lines.Add(new[]
                    {
                        $"{movementTokens[movement]} {position.ToString().PadLeft(2)}".Color(movementColors[movement]),
                        $" {currentEntry.UserMentionName}".Color(movementUserNameColors[movement]),
                        $" @{currentEntry.UserMentionScreenName}".Cyan(),
                        $" {currentEntry.Count:N0}".Color(movementColors[movement]),
                    });
                }

                Console.Clear();
                ColorConsole.WriteLine(
                   $"#{hashtag}".DarkCyan().On(ConsoleColor.White),
                   " mentions".Gray(),
                   $" · {DateTime.UtcNow}".DarkGray());

                ColorConsole.WriteLine(
                    "Powered by ".DarkGray(),
                    " NServiceBus ".White().OnDarkBlue(),
                    " from ".DarkGray(),
                    "Particular Software".White());

                foreach (var line in lines)
                {
                    ColorConsole.WriteLine(line);
                }

                ColorConsole.Write($"Total mentions: {hashtagMentions.Count:N0}".DarkGray());
                ColorConsole.WriteLine(initialCount.HasValue
                    ? $" ({(hashtagMentions.Count - initialCount) / (DateTime.UtcNow - start).TotalMinutes:N2} per minute)".DarkGray()
                    : string.Empty);

                var maxMessageLength = 0;
                var refreshTime = DateTime.UtcNow.AddMilliseconds(refreshInterval);
                using (var timer = new Timer(c =>
                {
                    var timeLeft = new TimeSpan(0, 0, 0, (int)Math.Round((refreshTime - DateTime.UtcNow).TotalSeconds));
                    var message = $"\rRefreshing in {timeLeft.Humanize()}...";
                    maxMessageLength = Math.Max(maxMessageLength, message.Length);
                    ColorConsole.Write(message.PadRight(maxMessageLength).DarkGray());
                }))
                {
                    timer.Change(0, 1000);
                    Thread.Sleep(refreshInterval);
                }

                initialCount = initialCount ?? hashtagMentions.Count;
                previousLeaderboard = currentLeaderboard;
            }
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
