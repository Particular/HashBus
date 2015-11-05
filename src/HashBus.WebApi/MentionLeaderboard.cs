namespace HashBus.WebApi
{
    using System.Collections.Generic;

    class MentionLeaderboard
    {
        public IList<Entry> Entries { get; set; }

        public int MentionsCount { get; set; }

        public class Entry
        {
            public long? UserMentionId { get; set; }

            public string UserMentionIdStr { get; set; }

            public string UserMentionName { get; set; }

            public string UserMentionScreenName { get; set; }

            public int Count { get; set; }
        }
    }
}
