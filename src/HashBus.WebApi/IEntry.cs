namespace HashBus.WebApi
{
    public interface IEntry
    {
        int Count { get; set; }

        int Position { get; set; }
    }
}
