using LUSID_InMemoryCache;

namespace LUSID_InMemoryCacheTests
{
    public class MemoryCacheTests
    {
        [Fact]
        public void Add_AddsItemToCache()
        {
            // Arrange
            IInMemoryCache<string, string> cache = new InMemoryCache<string, string>(1);

            // Act
            cache.Add("key", "value");

            // Assert
            Assert.True(cache.TryGetValue("key", out string value));
            Assert.Equal("value", value);
        }

        [Fact]
        public void Add_EvictsLeastRecentlyUsedItemWhenCacheIsFull()
        {
            // Arrange
            IInMemoryCache<string, string> cache = new InMemoryCache<string, string>(2);
            cache.Add("key1", "value1");
            cache.Add("key2", "value2");

            // Access key1 to make it the most recently used item
            cache.TryGetValue("key1", out _);

            // Act
            cache.Add("key3", "value3");

            // Assert
            Assert.False(cache.TryGetValue("key2", out _));
        }

        [Fact]
        public void TryGetValue_ReturnsFalseWhenItemIsNotFound()
        {
            // Arrange
            IInMemoryCache<string, string> cache = new InMemoryCache<string, string>(1);

            // Act
            bool result = cache.TryGetValue("key", out _);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ItemEvicted_IsRaisedWhenItemIsEvicted()
        {
            // Arrange
            IInMemoryCache<string, string> cache = new InMemoryCache<string, string>(1);
            cache.Add("key1", "value1");

            object evictedKey = null;
            object evictedValue = null;
            cache.ItemEvicted += (sender, e) =>
            {
                evictedKey = e.Key;
                evictedValue = e.Value;
            };

            // Act
            cache.Add("key2", "value2");

            // Assert
            Assert.Equal("key1", evictedKey);
            Assert.Equal("value1", evictedValue);
        }
    }
}