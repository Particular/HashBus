namespace HashBus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ColoredConsole;
    using HashBus.Application.Commands;

    class Writer
    {
        public static void Write(Tweet message)
        {
            var trackString = $" {message.Track} ";
            ColorConsole.WriteLine(
                new string(' ', (int)Math.Floor(Math.Max(0, Console.WindowWidth - trackString.Length - 1) / 2d)).OnDarkGray(),
                trackString.DarkCyan().OnWhite(),
                new string(' ', (int)Math.Ceiling(Math.Max(0, Console.WindowWidth - trackString.Length - 1) / 2d)).OnDarkGray());

            if (message.RetweetedTweet != null)
            {
                ColorConsole.WriteLine(
                    $"{message.RetweetedTweet.CreatedByName} ".White(),
                    $"@{message.RetweetedTweet.CreatedByScreenName} · ".DarkGray(),
                    $"{message.RetweetedTweet.CreatedAt.ToLocalTime()}".DarkGray(),
                    $" § {message.CreatedByName} Retweeted ".DarkGray());
            }
            else
            {
                ColorConsole.WriteLine(
                    $"{message.CreatedByName} ".White(),
                    $"@{message.CreatedByScreenName} · ".DarkGray(),
                    $"{message.CreatedAt.ToLocalTime()}".DarkGray());
            }

            var messageTokens = new List<ColorToken>();
            for (var index = 0; index < message.Text.Length; ++index)
            {
                var userMention = message.UserMentions.FirstOrDefault(m => m.Indices.First() == index);
                if (userMention == null)
                {
                    var hashTag = message.Hashtags.FirstOrDefault(m => m.Indices.First() == index);
                    if (hashTag == null)
                    {
                        messageTokens.Add(message.Text.Substring(index, 1).Gray());
                        continue;
                    }

                    var hashTaglength = hashTag.Indices.Last() - index;
                    messageTokens.Add(message.Text.Substring(index, hashTaglength).Cyan());

                    index += hashTaglength - 1;
                    continue;
                }

                var userMentionLength = userMention.Indices.Last() - index;
                messageTokens.Add(message.Text.Substring(index, userMentionLength).Cyan());
                index += userMentionLength - 1;
            }

            ColorConsole.WriteLine(messageTokens.ToArray());
        }
    }
}
