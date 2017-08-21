#if APPLICATION_EVENTS
namespace HashBus.Twitter.Analyzer.Events
#elif APPLICATION_COMMANDS
namespace HashBus.Twitter.Analyzer.Commands
#endif
{
    public class Hashtag
    {
        public string Text { get; set; }

        public int[] Indices { get; set; }
    }
}
