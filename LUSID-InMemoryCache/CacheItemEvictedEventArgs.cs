namespace LUSID_InMemoryCache
{
    public class CacheItemEvictedEventArgs : EventArgs
    {
        public object Key { get; private set; }
        public object? Value { get; private set; }

        public CacheItemEvictedEventArgs(object key, object? value)
        {
            Key = key;
            Value = value;
        }
    }
}
