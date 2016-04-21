namespace HashBus.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using LiteGuard;

    class IgnoredUserNamesService : IIgnoredUserNamesService
    {
        private readonly List<string> ignoredUserNames;

        public IgnoredUserNamesService(IEnumerable<string> ignoredUserNames)
        {
            Guard.AgainstNullArgument(nameof(ignoredUserNames), ignoredUserNames);

            this.ignoredUserNames = ignoredUserNames.ToList();
        }

        public IReadOnlyList<string> Get()
        {
            return this.ignoredUserNames.ToList();
        }
    }
}
