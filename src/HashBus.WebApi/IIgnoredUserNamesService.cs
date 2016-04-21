namespace HashBus.WebApi
{
    using System.Collections.Generic;

    public interface IIgnoredUserNamesService
    {
        IReadOnlyList<string> Get();
    }
}
