namespace HashBus.WebApi
{
    using System.Collections.Generic;

    class TweetLeaderboard
    {
        public IList<Entry> Entries { get; set; }

        public int TweetsCount { get; set; }

        public class Entry
        {
            public long UserId { get; set; }

            public string UserIdStr { get; set; }

            public string UserName { get; set; }

            public string UserScreenName { get; set; }

            public int Count { get; set; }
        }
    }
}
