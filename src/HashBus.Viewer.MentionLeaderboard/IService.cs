using System.Threading.Tasks;

namespace HashBus.Viewer
{
    interface IService<in TKey, TValue>
    {
        Task<TValue> GetAsync(TKey key);
    }
}
