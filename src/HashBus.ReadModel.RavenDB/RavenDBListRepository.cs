using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteGuard;
using Raven.Client;

namespace HashBus.ReadModel.RavenDB
{
    public class RavenDBListRepository<TValue> : IRepository<string, IEnumerable<TValue>>
    {
        private readonly IDocumentStore store;

        public RavenDBListRepository(IDocumentStore store)
        {
            Guard.AgainstNullArgument(nameof(store), store);

            this.store = store;
        }

        public async Task<IEnumerable<TValue>> GetAsync(string key)
        {
            using (var session = this.store.OpenAsyncSession())
            {
                return (await session.LoadAsync<Document>(key))?.Values ?? Enumerable.Empty<TValue>();
            }
        }

        public async Task SaveAsync(string key, IEnumerable<TValue> value)
        {
            using (var session = this.store.OpenAsyncSession())
            {
                await session.StoreAsync(new Document { Values = value }, key);
                await session.SaveChangesAsync();
            }
        }

        private class Document
        {
            public IEnumerable<TValue> Values { get; set; }
        }
    }
}
