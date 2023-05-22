namespace LUSID_InMemoryCache
{
    public interface IInMemoryCache<TKey, TValue>
    {
        event EventHandler<CacheItemEvictedEventArgs> ItemEvicted;
        void Add(TKey key, TValue value);
        bool TryGetValue(TKey key, out TValue value);
    }
}
