namespace HashBus.Viewer
{
    using System.Threading.Tasks;

    interface IService<in TKey, TValue>
    {
        Task<TValue> GetAsync(TKey key);
    }
}
