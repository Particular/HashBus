namespace HashBus.WebApi
{
    using System.Collections.Generic;
    using System.Linq;
    using LiteGuard;

    class IgnoredHashtagsService : IIgnoredHashtagsService
    {
        private readonly List<string> ignoredHashtags;

        public IgnoredHashtagsService(IEnumerable<string> ignoredHashtags)
        {
            Guard.AgainstNullArgument(nameof(ignoredHashtags), ignoredHashtags);

            this.ignoredHashtags = ignoredHashtags.ToList();
        }

        public IReadOnlyList<string> Get()
        {
            return this.ignoredHashtags.ToList();
        }
    }
}
