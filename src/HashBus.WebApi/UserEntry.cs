namespace HashBus.WebApi
{
    public class UserEntry : IEntry
    {
        public int Position { get; set; }

        public long? Id { get; set; }

        public string IdStr { get; set; }

        public string Name { get; set; }

        public string ScreenName { get; set; }

        public int Count { get; set; }
    }
}
