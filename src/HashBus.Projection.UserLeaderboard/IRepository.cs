using System.Threading.Tasks;

namespace HashBus.Projection.UserLeaderboard
{
    public interface IRepository<TKey, TValue>
    {
        Task<TValue> GetAsync(TKey key);

        Task SaveAsync(TKey key, TValue value);
    }
}
