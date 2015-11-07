namespace HashBus.Viewer
{
    using System.Collections.Generic;
    using System.Linq;
    using ColoredConsole;

    static class ColorTokenExtensions
    {
        public static IList<ColorToken> Trim(this IEnumerable<ColorToken> source, int maxWidth)
        {
            IList<ColorToken> tokens = new List<ColorToken>(source);
            while (tokens.Sum(token => token.Text.Length) > maxWidth)
            {
                tokens = tokens
                    .Reverse()
                    .SkipWhile(token => token.Text.Length == 0)
                    .Reverse()
                    .ToList();

                var oldLastToken = tokens.Last();
                var newLastToken = new ColorToken(
                    oldLastToken.Text.Substring(0, oldLastToken.Text.Length - 1),
                    oldLastToken.Color,
                    oldLastToken.BackgroundColor);

                tokens = tokens.Take(tokens.Count - 1).ToList();
                tokens.Add(newLastToken);
            }

            return tokens;
        }
    }
}
