namespace HashBus.ReadModel
{
    using System.Threading.Tasks;

    public interface IRepository<in TKey, TValue>
    {
        Task<TValue> GetAsync(TKey key);

        Task SaveAsync(TKey key, TValue value);
    }
}
