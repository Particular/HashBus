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
            ColorConsole.WriteLine(
                $"{message.TweetCreatedAt} ".DarkCyan(),
                message.TweetIsRetweet ? "Retweet by ".DarkGreen() : "Tweet by ".Green(),
                $"{message.TweetCreatedByName} ".Yellow(),
                $"@{message.TweetCreatedByScreenName}".DarkYellow());

            var messageTokens = new List<ColorToken> { "  " };
            for (var index = 0; index < message.TweetText.Length; ++index)
            {
                var userMention = message.TweetUserMentions.FirstOrDefault(m => m.Indices.First() == index);
                if (userMention == null)
                {
                    messageTokens.Add(message.TweetText.Substring(index, 1).White());
                    continue;
                }

                var length = userMention.Indices.Last() - index;
                messageTokens.Add(message.TweetText.Substring(index, length).Cyan());
                index += length - 1;
            }

            ColorConsole.WriteLine(messageTokens.ToArray());
        }
    }
}
