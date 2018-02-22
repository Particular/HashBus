namespace HashBus.Viewer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using ColoredConsole;

    class LeaderboardView<TEntry> : IRunAsync where TEntry : WebApi.IEntry
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

        private readonly string track;
        private readonly int refreshInterval;
        private readonly IService<string, WebApi.Leaderboard<TEntry>> leaderboards;
        private readonly bool showPercentages;
        private readonly int verticalPadding;
        private readonly int horizontalPadding;
        private readonly Func<TEntry, TEntry, bool> matchEntries;
        private readonly Func<TEntry, IEnumerable<ColorToken>> getText;
        private readonly string name;
        private readonly string itemsName;

        private WebApi.Leaderboard<TEntry> previousLeaderboard = new WebApi.Leaderboard<TEntry>();

        public LeaderboardView(
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
            this.track = track;
            this.refreshInterval = refreshInterval;
            this.leaderboards = leaderboards;
            this.showPercentages = showPercentages;
            this.verticalPadding = verticalPadding;
            this.horizontalPadding = horizontalPadding;
            this.matchEntries = matchEntries;
            this.getText = getText;
            this.name = name;
            this.itemsName = itemsName;
        }

        public Task RunAsync()
        {
            return this.RunAsync(new CancellationTokenSource().Token);
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            Console.CursorVisible = false;
            while (!cancellationToken.IsCancellationRequested)
            {
                WebApi.Leaderboard<TEntry> currentLeaderboard;
                try
                {
                    currentLeaderboard = await leaderboards.GetAsync(track);
                }
                catch (Exception)
                {
                    currentLeaderboard = previousLeaderboard;
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
                ColorConsole.Write(
                    padding,
                    $" {track} ".DarkCyan().On(ConsoleColor.White),
                    " ",
                    name.White());

                if (currentLeaderboard == previousLeaderboard)
                {
                    ColorConsole.Write(" ", currentLeaderboard == previousLeaderboard ? " Service unavailable ".Yellow().OnRed() : "");
                }

                ColorConsole.WriteLine();

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

                ColorConsole.WriteLine();

                var totalColor = currentLeaderboard?.Count - previousLeaderboard?.Count > 0 ? movementColors[-1] : movementColors[0];
                ColorConsole.Write(padding, $"{currentLeaderboard?.Count ?? 0:N0} ".Color(totalColor), $"{itemsName}".DarkGray());

                if (currentLeaderboard.Since.HasValue)
                {
                    ColorConsole.WriteLine(
                        $" since ".DarkGray(),
                        $"{currentLeaderboard.Since?.ToLocalTime():dddd} {currentLeaderboard.Since?.ToLocalTime():HH:mm}".Gray());
                }

                var maxMessageLength = 0;
                var refreshTime = DateTime.UtcNow.AddMilliseconds(refreshInterval);
                using (var timer = new Timer(c =>
                {
                    var timeLeft = new TimeSpan(0, 0, 0, (int)Math.Round((refreshTime - DateTime.UtcNow).TotalSeconds));
                    if (timeLeft.TotalSeconds == 0)
                    {
                        return;
                    }

                    var tokens = new[]
                    {
                        $"\r{padding}github.com/Particular/HashBus".Cyan(),
                        $" · Refreshing in {timeLeft.TotalSeconds}...".DarkGray(),
                    };

                    var currentLength = tokens.Sum(x => x.Text.Length);
                    maxMessageLength = Math.Max(maxMessageLength, currentLength);
                    tokens = tokens.Concat(new ColorToken[] { new string(' ', maxMessageLength - currentLength) }).ToArray();
                    ColorConsole.Write(tokens);
                }))
                {
                    timer.Change(0, 1000);
                    await Task.Delay(refreshInterval);
                }

                previousLeaderboard = currentLeaderboard;
            }
        }
    }
}
