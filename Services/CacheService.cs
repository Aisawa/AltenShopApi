using Microsoft.Extensions.Caching.Memory;

namespace AltenShopApi.Services
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (_memoryCache.TryGetValue(key, out T cachedValue))
            {
                _logger.LogInformation("Cache hit for key: {Key}", key);
                value = cachedValue;
                return true;
            }

            _logger.LogInformation("Cache miss for key: {Key}", key);
            value = default;
            return false;
        }

        public void Set<T>(string key, T value, TimeSpan expiration)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            _memoryCache.Set(key, value, options);
            _logger.LogInformation("Added to cache: {Key}, expires in {Expiration}", key, expiration);
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
            _logger.LogInformation("Removed from cache: {Key}", key);
        }
    }
}
