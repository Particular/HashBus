namespace HashBus.Viewer.TweetLeaderboard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ColoredConsole;
    using Humanizer;

    class TweetLeaderboardView
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
            string track, int refreshInterval, IService<string, WebApi.TweetLeaderboard> leaderboards, bool showPercentages)
        {
            var previousLeaderboard = new WebApi.TweetLeaderboard();
            while (true)
            {
                WebApi.TweetLeaderboard currentLeaderboard;
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
                    Enumerable.Empty<WebApi.TweetLeaderboard.Entry>())
                {
                    ++position;
                    var previousEntry = (previousLeaderboard?.Entries ??
                            Enumerable.Empty<WebApi.TweetLeaderboard.Entry>())
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

                    var tokens = new List<ColorToken>
                    {
                        $"{movementTokens[movement]} {position.ToString().PadLeft(2)}".Color(movementColors[movement]),
                        $" {currentEntry.UserName}".White(),
                        $" @{currentEntry.UserScreenName}".Cyan(),
                        $" {currentEntry.Count:N0}".Color(movementColors[countMovement]),
                    };

                    if (showPercentages)
                    {
                        tokens.Add($" ({currentEntry.Count / (double)currentLeaderboard.TweetsCount:P0})".DarkGray());
                    }

                    tokens.Add(new string(' ', Math.Max(0, Console.WindowWidth - 1 - tokens.Sum(token => token.Text.Length))));

                    lines.Add(tokens.Select(token => token.On(movementBackgroundColors[movement])).ToArray());
                }

                Console.Clear();
                ColorConsole.WriteLine(
                    $" {track} ".DarkCyan().On(ConsoleColor.White),
                    " Top Tweeters".White());

                ColorConsole.WriteLine(
                    "Powered by ".DarkGray(),
                    " NServiceBus ".White().OnDarkBlue(),
                    " from ".DarkGray(),
                    "Particular Software".White());

                ColorConsole.WriteLine();
                foreach (var line in lines)
                {
                    ColorConsole.WriteLine(line);
                }

                ColorConsole.WriteLine(
                    "Total tweets:".Gray(),
                    " ",
                    $"{currentLeaderboard?.TweetsCount ?? 0:N0}"
                        .Color(currentLeaderboard?.TweetsCount - previousLeaderboard?.TweetsCount > 0 ? movementColors[-1] : movementColors[0]),
                    $" · {DateTime.UtcNow.ToLocalTime()}".DarkGray());

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

                previousLeaderboard = currentLeaderboard;
            }
        }
    }
}
