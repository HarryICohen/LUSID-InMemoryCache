using LUSID_InMemoryCache;

// Create a cache that can hold a maximum of 20 items
IInMemoryCache<string, string> cache = new InMemoryCache<string, string>(20);
// Subscribe to the eviction event
cache.ItemEvicted += OnItemEvicted;

for (int i = 1; i < 25; i++)
{
    cache.Add("key" + i, "value" + i);
}

FindInCache(cache, "key1"); // will fail as only 20 items in cache
FindInCache(cache, "key10");
FindInCache(cache, "key20");

// Unsubscribe from the eviction event
cache.ItemEvicted -= OnItemEvicted;

Console.WriteLine("Press a key to exit");
Console.ReadKey();

void OnItemEvicted(object sender, CacheItemEvictedEventArgs e)
{
    if (e != null)
    {
        // Handle the eviction event here
        Console.WriteLine($"Item with key ({e.Key}) was evicted from the cache.");
    }
}

void FindInCache(IInMemoryCache<string, string> cache, string key)
{
    // Check value exist
    if (cache.TryGetValue(key, out string value))
    {
        Console.WriteLine($"Key ({key}) found in cache with value ({value}).");
    }
    else
    {
        Console.WriteLine($"Key ({key}) not found");
    }
}
