namespace HashBus.Twitter.Monitor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ColoredConsole;
    using HashBus.Twitter.Events;

    class Writer
    {
        public static void Write(TweetReceived message)
        {
            var trackString = $" {message.Track} ";
            ColorConsole.WriteLine(
                new string(' ', (int)Math.Floor(Math.Max(0, Console.WindowWidth - trackString.Length - 1) / 2d)).OnDarkGray(),
                trackString.DarkCyan().OnWhite(),
                new string(' ', (int)Math.Ceiling(Math.Max(0, Console.WindowWidth - trackString.Length - 1) / 2d)).OnDarkGray());

            if (message.TweetIsRetweet)
            {
                ColorConsole.WriteLine(
                    $"{message.RetweetedTweetCreatedByName} ".White(),
                    $"@{message.RetweetedTweetCreatedByScreenName} · ".DarkGray(),
                    $"{message.RetweetedTweetCreatedAt.ToLocalTime()}".DarkGray(),
                    $" § {message.TweetCreatedByName} Retweeted ".DarkGray());
            }
            else
            {
                ColorConsole.WriteLine(
                    $"{message.TweetCreatedByName} ".White(),
                    $"@{message.TweetCreatedByScreenName} · ".DarkGray(),
                    $"{message.TweetCreatedAt.ToLocalTime()}".DarkGray());
            }

            var messageTokens = new List<ColorToken>();
            for (var index = 0; index < message.TweetText.Length; ++index)
            {
                var userMention = message.TweetUserMentions.FirstOrDefault(m => m.Indices.First() == index);
                if (userMention == null)
                {
                    var hashTag = message.TweetHashtags.FirstOrDefault(m => m.Indices.First() == index);
                    if (hashTag == null)
                    {
                        messageTokens.Add(message.TweetText.Substring(index, 1).Gray());
                        continue;
                    }

                    var hashTaglength = hashTag.Indices.Last() - index;
                    messageTokens.Add(message.TweetText.Substring(index, hashTaglength).Cyan());

                    index += hashTaglength - 1;
                    continue;
                }

                var userMentionLength = userMention.Indices.Last() - index;
                messageTokens.Add(message.TweetText.Substring(index, userMentionLength).Cyan());
                index += userMentionLength - 1;
            }

            ColorConsole.WriteLine(messageTokens.ToArray());
        }
    }
}
