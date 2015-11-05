using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ColoredConsole;
using Humanizer;

namespace HashBus.Viewer.MentionLeaderboard
{
    class MentionLeaderboardView
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
            string track, int refreshInterval, IService<string, WebApi.MentionLeaderboard> leaderboards, bool showPercentages)
        {
            var start = DateTime.UtcNow;
            var initialCount = (int?)null;
            var previousLeaderboard = new WebApi.MentionLeaderboard();
            while (true)
            {
                WebApi.MentionLeaderboard currentLeaderboard;
                try
                {
                    currentLeaderboard = await leaderboards.GetAsync(track);
                }
                catch (Exception ex)
                {
                    ColorConsole.WriteLine("Failed to get leaderboard. ".Red(), ex.Message.DarkRed());
                    Thread.Sleep(1000);
                    continue;
                }

                var position = 0;
                var lines = new List<ColorToken[]>();
                foreach (var currentEntry in currentLeaderboard?.Entries ??
                    Enumerable.Empty<WebApi.MentionLeaderboard.Entry>())
                {
                    ++position;
                    var previousEntry = (previousLeaderboard?.Entries ??
                            Enumerable.Empty<WebApi.MentionLeaderboard.Entry>())
                        .Select((entry, index) => new
                        {
                            Entry = entry,
                            Position = index + 1
                        })
                        .FirstOrDefault(e => e.Entry.UserMentionId == currentEntry.UserMentionId);

                    var movement = previousEntry == null
                        ? int.MinValue
                        : Math.Sign(position - previousEntry.Position);

                    var countMovement = Math.Sign(Math.Min(previousEntry?.Entry.Count - currentEntry.Count ?? 0, movement));

                    lines.Add(new[]
                    {
                        $"{movementTokens[movement]} {position.ToString().PadLeft(2)}".Color(movementColors[movement]).On(movementBackgroundColors[movement]),
                        $" {currentEntry.UserMentionName}".White().On(movementBackgroundColors[movement]),
                        $" @{currentEntry.UserMentionScreenName}".Cyan().On(movementBackgroundColors[movement]),
                        $" {currentEntry.Count:N0}".Color(movementColors[countMovement]).On(movementBackgroundColors[movement]),
                        showPercentages ? $" ({currentEntry.Count / (double)currentLeaderboard.MentionsCount:P0})".DarkGray().On(movementBackgroundColors[movement]) : null,
                    });
                }

                Console.Clear();
                ColorConsole.WriteLine(
                    $" {track} ".DarkCyan().On(ConsoleColor.White),
                    " mentions".Gray(),
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

                ColorConsole.Write($"Total mentions: {currentLeaderboard?.MentionsCount ?? 0:N0}".DarkGray());
                ColorConsole.WriteLine(initialCount.HasValue
                    ? $" ({(currentLeaderboard?.MentionsCount - initialCount) / (DateTime.UtcNow - start).TotalMinutes:N2} per minute)".DarkGray()
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

                initialCount = initialCount ?? currentLeaderboard?.MentionsCount;
                previousLeaderboard = currentLeaderboard;
            }
        }
    }
}
