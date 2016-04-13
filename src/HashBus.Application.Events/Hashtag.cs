#if APPLICATION_EVENTS
namespace HashBus.Application.Events
#elif APPLICATION_COMMANDS
namespace HashBus.Application.Commands
#endif
{
    public class Hashtag
    {
        public string Text { get; set; }

        public int[] Indices { get; set; }
    }
}
