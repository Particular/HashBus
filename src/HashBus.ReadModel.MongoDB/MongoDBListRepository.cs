using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using global::MongoDB.Bson.Serialization.Attributes;
using LiteGuard;
using MongoDB.Driver;

namespace HashBus.ReadModel.MongoDB
{
    public class MongoDBListRepository<TValue> : IRepository<string, IEnumerable<TValue>>
    {
        private readonly IMongoCollection<Document> collection;

        public MongoDBListRepository(IMongoDatabase database, string collectionName)
        {
            Guard.AgainstNullArgument(nameof(database), database);

            this.collection = database.GetCollection<Document>(collectionName);
        }

        public async Task<IEnumerable<TValue>> GetAsync(string key)
        {
            key = key?.ToLowerInvariant();
            return (await this.collection.Find(doc => doc.Id == key).ToListAsync()).FirstOrDefault()?.Values
                ?? Enumerable.Empty<TValue>();
        }

        public async Task SaveAsync(string key, IEnumerable<TValue> value)
        {
            key = key?.ToLowerInvariant();
            await this.collection.ReplaceOneAsync(
                doc => doc.Id == key, new Document { Id = key, Values = value }, new UpdateOptions { IsUpsert = true });
        }

        private class Document
        {
            [BsonId]
            public string Id { get; set; }

            public IEnumerable<TValue> Values { get; set; }
        }
    }
}
