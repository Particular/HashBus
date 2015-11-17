namespace HashBus.WebApi
{
    using System.Collections.Generic;

    class Leaderboard<TEntry>
    {
        public IList<TEntry> Entries { get; set; }

        public int Count { get; set; }
    }
}
