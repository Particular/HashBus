namespace HashBus.WebApi
{
    using System.Collections.Generic;

    public interface IIgnoredHashtagsService
    {
        IReadOnlyList<string> Get();
    }
}
