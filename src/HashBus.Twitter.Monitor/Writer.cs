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
                $"{message.CreatedAt} ".DarkCyan(),
                message.IsRetweet ? "Retweet by ".DarkGreen() : "Tweet by ".Green(),
                $"{message.UserName} ".Yellow(),
                $"@{message.UserScreenName}".DarkYellow());

            var messageTokens = new List<ColorToken> { "  " };
            for (var index = 0; index < message.Text.Length; ++index)
            {
                var userMention = message.UserMentions.FirstOrDefault(m => m.Indices.First() == index);
                if (userMention == null)
                {
                    messageTokens.Add(message.Text.Substring(index, 1).White());
                    continue;
                }

                var length = userMention.Indices.Last() - index;
                messageTokens.Add(message.Text.Substring(index, length).Cyan());
                index += length - 1;
            }

            ColorConsole.WriteLine(messageTokens.ToArray());
        }
    }
}
