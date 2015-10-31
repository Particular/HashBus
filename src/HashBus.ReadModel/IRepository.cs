using System.Threading.Tasks;

namespace HashBus.ReadModel
{
    public interface IRepository<TKey, TValue>
    {
        Task<TValue> GetAsync(TKey key);

        Task SaveAsync(TKey key, TValue value);
    }
}
