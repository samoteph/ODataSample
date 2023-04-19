using Microsoft.Extensions.Caching.Memory;

namespace Scamark.Microservice.Cache;

public class MemoryCache<TItem> : IDisposable
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions()
    {
        SizeLimit = 1024
    });

    public TItem GetOrCreate(object key, Func<TItem> createItem)
    {
        if (!_cache.TryGetValue(key, out TItem cacheEntry))// Look for cache key.
        {
            // Key not in cache, so get data.
            cacheEntry = createItem();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(1)     //Size amount
                                //Priority on removing when reaching size limit (memory pressure)
                .SetPriority(CacheItemPriority.High)
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                // Remove from cache after this time, regardless of sliding expiration
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(60));

            // Save data in cache.
            _cache.Set(key, cacheEntry, cacheEntryOptions);
        }
        return cacheEntry;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _cache.Dispose();
    }
}
