namespace HashBus.WebApi
{
    using System;
    using System.Collections.Generic;

    class Leaderboard<TEntry>
    {
        public IList<TEntry> Entries { get; set; }

        public int Count { get; set; }

        public DateTime? Since { get; set; }

        public DateTime? LastActivityDateTime { get; set; }
    }
}
