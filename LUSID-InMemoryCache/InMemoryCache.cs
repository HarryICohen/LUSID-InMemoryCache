namespace LUSID_InMemoryCache
{
    public class InMemoryCache<TKey, TValue> : IInMemoryCache<TKey, TValue> where TKey : notnull
    {
        private readonly Dictionary<TKey, CacheItem> _cache;
        private readonly int _maxItems;                     // user configurable cache size.
        private readonly ReaderWriterLockSlim _lock;        // use lock to ensure thread safe.

        public event EventHandler<CacheItemEvictedEventArgs>? ItemEvicted;   // Event to subscribe to for evictions

        public InMemoryCache(int maxItems)
        {
            _maxItems = maxItems;
            _cache = new Dictionary<TKey, CacheItem>(_maxItems);
            _lock = new ReaderWriterLockSlim();
        }

        public void Add(TKey key, TValue value)
        {
            _lock.EnterWriteLock();
            try
            {
                // See if reached cache size, and evict using ‘least recently used’ approach.
                if (_cache.Count == _maxItems)
                {
                    TKey keyToRemove = default!;
                    DateTime oldestAccessTime = DateTime.MaxValue;

                    foreach (KeyValuePair<TKey, CacheItem> c in _cache)
                    {
                        if (c.Value.LastAccessTime < oldestAccessTime)
                        {
                            oldestAccessTime = c.Value.LastAccessTime;
                            keyToRemove = c.Key;
                        }
                    }

                    if (keyToRemove != null)
                    {
                        TValue removedValue = _cache[keyToRemove].Value;
                        _cache.Remove(keyToRemove);
                        // Raise event to consumer of cache
                        OnItemEvicted(new CacheItemEvictedEventArgs(keyToRemove, removedValue));
                    }
                }

                _cache[key] = new CacheItem(value);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default!;
            bool result = false;

            _lock.EnterUpgradeableReadLock();
            try
            {
                CacheItem? cacheItem;
                if (_cache.TryGetValue(key, out cacheItem))
                {
                    value = cacheItem.Value;
                    cacheItem.LastAccessTime = DateTime.UtcNow;
                    result = true;
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }

            return result;
        }

        #region CacheEviction
        /// <summary>
        /// Cache events for eviction 
        /// </summary>
        protected virtual void OnItemEvicted(CacheItemEvictedEventArgs e)
        {
            ItemEvicted?.Invoke(this, e);
        }

        private class CacheItem
        {
            public TValue Value { get; private set; }
            public DateTime LastAccessTime { get; set; }

            public CacheItem(TValue value)
            {
                Value = value;
                LastAccessTime = DateTime.UtcNow;
            }
        }
        #endregion
        
    }
}
