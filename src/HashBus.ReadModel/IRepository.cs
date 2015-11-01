using System.Threading.Tasks;

namespace HashBus.ReadModel
{
    public interface IRepository<in TKey, TValue>
    {
        Task<TValue> GetAsync(TKey key);

        Task SaveAsync(TKey key, TValue value);
    }
}
