#if APPLICATION_EVENTS
namespace HashBus.Application.Events
#elif APPLICATION_COMMANDS
namespace HashBus.Application.Commands
#endif
{
    using System.Collections.Generic;

    public class UserMention
    {
        public long Id { get; set; }

        public string IdStr { get; set; }

        public IList<int> Indices { get; set; }

        public string Name { get; set; }

        public string ScreenName { get; set; }
    }
}
