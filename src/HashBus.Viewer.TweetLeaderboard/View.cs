using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ColoredConsole;
using HashBus.ReadModel;
using Humanizer;

namespace HashBus.Viewer.TweetLeaderboard
{
    class View
    {
        private static readonly Dictionary<int, string> movementTokens =
            new Dictionary<int, string>
            {
                { int.MinValue, ">" },
                { -1, "^" },
                { 0, "=" },
                { 1, "v" },
            };

        private static readonly Dictionary<int, ConsoleColor> movementColors =
            new Dictionary<int, ConsoleColor>
            {
                { int.MinValue, ConsoleColor.Yellow },
                { -1, ConsoleColor.Green},
                { 0, ConsoleColor.Gray },
                { 1, ConsoleColor.Red },
            };

        private static readonly Dictionary<int, ConsoleColor> movementBackgroundColors =
            new Dictionary<int, ConsoleColor>
            {
                { int.MinValue, ConsoleColor.DarkYellow },
                { -1, ConsoleColor.DarkGreen },
                { 0, ConsoleColor.Black },
                { 1, ConsoleColor.DarkRed },
            };

        public static async Task StartAsync(
            string hashtag, int refreshInterval, IRepository<string, IEnumerable<Tweet>> tweets, bool showPercentages)
        {
            var start = DateTime.UtcNow;
            var initialCount = (int?)null;
            var previousLeaderboard = new List<Entry>();
            while (true)
            {
                var hashtagTweets = (await tweets.GetAsync(hashtag)).ToList();
                var currentLeaderboard = hashtagTweets
                    .GroupBy(tweet => tweet.UserId)
                    .Select(g => new Entry
                    {
                        UserId = g.Key,
                        UserIdStr = g.First().UserIdStr,
                        UserName = g.First().UserName,
                        UserScreenName = g.First().UserScreenName,
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
                        .Select((entry, index) => new
                        {
                            Entry = entry,
                            Position = index + 1
                        })
                        .FirstOrDefault(e => e.Entry.UserId == currentEntry.UserId);

                    var movement = previousEntry == null
                        ? int.MinValue
                        : Math.Sign(position - previousEntry.Position);

                    var countMovement = Math.Sign(Math.Min(previousEntry?.Entry.Count - currentEntry.Count ?? 0, movement));

                    lines.Add(new[]
                    {
                        $"{movementTokens[movement]} {position.ToString().PadLeft(2)}".Color(movementColors[movement]).On(movementBackgroundColors[movement]),
                        $" {currentEntry.UserName}".White().On(movementBackgroundColors[movement]),
                        $" @{currentEntry.UserScreenName}".Cyan().On(movementBackgroundColors[movement]),
                        $" {currentEntry.Count:N0}".Color(movementColors[countMovement]).On(movementBackgroundColors[movement]),
                        showPercentages ? $" ({currentEntry.Count / (double)hashtagTweets.Count:P0})".DarkGray().On(movementBackgroundColors[movement]) : null,
                    });
                }

                Console.Clear();
                ColorConsole.WriteLine(
                    $"#{hashtag}".DarkCyan().On(ConsoleColor.White),
                    " tweets".Gray(),
                    $" · {DateTime.UtcNow.ToLocalTime()}".DarkGray());

                ColorConsole.WriteLine(
                    "Powered by ".DarkGray(),
                    " NServiceBus ".White().OnDarkBlue(),
                    " from ".DarkGray(),
                    "Particular Software".White());

                foreach (var line in lines)
                {
                    ColorConsole.WriteLine(line);
                }

                ColorConsole.Write($"Total tweets: {hashtagTweets.Count:N0}".DarkGray());
                ColorConsole.WriteLine(initialCount.HasValue
                    ? $" ({(hashtagTweets.Count - initialCount) / (DateTime.UtcNow - start).TotalMinutes:N2} per minute)".DarkGray()
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

                initialCount = initialCount ?? hashtagTweets.Count;
                previousLeaderboard = currentLeaderboard;
            }
        }

        class Entry
        {
            public long? UserId { get; set; }

            public string UserIdStr { get; set; }

            public string UserName { get; set; }

            public string UserScreenName { get; set; }

            public int Count { get; set; }
        }
    }
}
