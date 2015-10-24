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

            ColorConsole.WriteLine($"  {message.Text}".White());
        }
    }
}
