using System;
using System.Collections.Generic;
using System.Linq;
using ColoredConsole;
using HashBus.Twitter.Events;

namespace HashBus.Twitter.Monitor
{
    class Writer
    {
        public static void Write(HashtagTweeted message)
        {
            if (message.TweetIsRetweet)
            {
                ColorConsole.WriteLine(
                    "» ".DarkGray(),
                    $"{message.RetweetedTweetCreatedByName} ".White(),
                    $"{message.RetweetedTweetCreatedByScreenName} · ".DarkGray(),
                    $"{message.RetweetedTweetCreatedAt}".DarkGray(),
                    $" § {message.TweetCreatedByName} Retweeted ".DarkGray());
            }
            else
            {
                ColorConsole.WriteLine(
                    "» ".DarkGray(),
                    $"{message.TweetCreatedByName} ".White(),
                    $"{message.TweetCreatedByScreenName} · ".DarkGray(),
                    $"{message.TweetCreatedAt}".DarkGray());
            }

            var messageTokens = new List<ColorToken> { "  " };
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
                    messageTokens.Add(
                        message.TweetText.Substring(index, hashTaglength)
                        .Color(hashTag.Text.Equals(message.Hashtag, StringComparison.OrdinalIgnoreCase)
                            ? ConsoleColor.DarkCyan
                            : ConsoleColor.Cyan)
                            .On(hashTag.Text.Equals(message.Hashtag, StringComparison.OrdinalIgnoreCase)
                            ? ConsoleColor.White
                            : (ConsoleColor?)null));

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
