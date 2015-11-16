namespace HashBus.WebApi
{
    public class HashtagEntry : IEntry
    {
        public int Position { get; set; }

        public string Text { get; set; }

        public int Count { get; set; }
    }
}
