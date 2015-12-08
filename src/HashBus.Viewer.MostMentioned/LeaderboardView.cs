namespace HashBus.Viewer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ColoredConsole;
    using Humanizer;

    static class LeaderboardView<TEntry> where TEntry : WebApi.IEntry
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
            string track,
            int refreshInterval,
            IService<string, WebApi.Leaderboard<TEntry>> leaderboards,
            bool showPercentages,
            int verticalPadding,
            int horizontalPadding,
            Func<TEntry, TEntry, bool> matchEntries,
            Func<TEntry, IEnumerable<ColorToken>> getText,
            string name,
            string itemsName)
        {
            Console.CursorVisible = false;
            var previousLeaderboard = new WebApi.Leaderboard<TEntry>();
            while (true)
            {
                WebApi.Leaderboard<TEntry> currentLeaderboard;
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

                var lines = new List<IEnumerable<ColorToken>>();
                foreach (var currentEntry in currentLeaderboard?.Entries ??
                    Enumerable.Empty<TEntry>())
                {
                    var previousEntry = (previousLeaderboard?.Entries ??
                            Enumerable.Empty<TEntry>())
                        .FirstOrDefault(e => matchEntries(e, currentEntry));

                    var movement = previousEntry == null
                        ? int.MinValue
                        : Math.Sign(currentEntry.Position - previousEntry.Position);

                    var countMovement = Math.Sign(Math.Min(previousEntry?.Count - currentEntry.Count ?? 0, movement));

                    var tokens = new List<ColorToken>
                    {
                        $"{movementTokens[movement]} {currentEntry.Position.ToString().PadLeft(2)}".Color(movementColors[movement]),
                    };

                    tokens.AddRange(getText(currentEntry));
                    tokens.Add($" {currentEntry.Count:N0}".Color(movementColors[countMovement]));

                    if (showPercentages)
                    {
                        tokens.Add($" ({currentEntry.Count / (double)currentLeaderboard.Count:P0})".DarkGray());
                    }

                    var maxWidth = Console.WindowWidth - (horizontalPadding * 2);
                    tokens.Add(new string(' ', Math.Max(0, maxWidth - tokens.Sum(token => token.Text.Length))));

                    lines.Add(tokens.Trim(maxWidth).Select(token => token.On(movementBackgroundColors[movement])));
                }

                Console.Clear();
                for (var newLine = verticalPadding - 1; newLine >= 0; newLine--)
                {
                    ColorConsole.WriteLine();
                }

                var padding = new string(' ', horizontalPadding);
                ColorConsole.WriteLine(
                    padding,
                    $" {track} ".DarkCyan().On(ConsoleColor.White),
                    " ",
                    name.White());

                ColorConsole.WriteLine(
                    padding,
                    "Powered by ".DarkGray(),
                    " NServiceBus ".White().OnDarkBlue(),
                    " from ".DarkGray(),
                    "Particular Software".White());

                ColorConsole.WriteLine();
                foreach (var line in lines)
                {
                    ColorConsole.WriteLine(new ColorToken[] { padding }.Concat(line).ToArray());
                }

                if (currentLeaderboard.Since.HasValue)
                {
                    ColorConsole.WriteLine(
                        padding,
                        $"{currentLeaderboard.Since?.ToLocalTime()} to {currentLeaderboard.LastActivityDateTime?.ToLocalTime()}".Gray());
                }

                var totalColor = currentLeaderboard?.Count - previousLeaderboard?.Count > 0 ? movementColors[-1] : movementColors[0];
                var maxMessageLength = 0;
                var refreshTime = DateTime.UtcNow.AddMilliseconds(refreshInterval);
                using (var timer = new Timer(c =>
                {
                    var timeLeft = new TimeSpan(0, 0, 0, (int)Math.Round((refreshTime - DateTime.UtcNow).TotalSeconds));

                    var tokens = new []
                    {
                        $"\r{padding}Total {itemsName}: ".DarkGray(),
                        $"{currentLeaderboard?.Count ?? 0:N0}".Color(totalColor),
                        $" · Refreshing in {timeLeft.Humanize()}...".DarkGray()
                    };

                    var currentLength = tokens.Sum(x => x.Text.Length);
                    maxMessageLength = Math.Max(maxMessageLength, currentLength);
                    tokens = tokens.Concat(new ColorToken[] { new string(' ', maxMessageLength - currentLength) }).ToArray();
                    ColorConsole.Write(tokens);
                }))
                {
                    timer.Change(0, 1000);
                    Thread.Sleep(refreshInterval);
                }

                previousLeaderboard = currentLeaderboard;
            }
        }
    }
}
