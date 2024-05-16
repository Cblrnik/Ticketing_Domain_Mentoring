using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Ticketing.Caching.Services
{
    public class InMemoryCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _distributedCache;

        public InMemoryCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task CacheResponseAsync(string key, object? value, uint timeToLiveInSeconds = 120)
        {
            if (value is null)
            {
                return;
            }

            var timeToLive = TimeSpan.FromSeconds(timeToLiveInSeconds);
            var memory = new MemoryStream();
            await JsonSerializer.SerializeAsync(memory, value);
            memory.Position = 0;
            var jsonData = await new StreamReader(memory).ReadToEndAsync();
            await _distributedCache.SetStringAsync(key, jsonData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeToLive,
            });
        }

        public async Task DeleteByKey(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        public async Task<string> GetCachedResponseAsync(string key)
        {
            var jsonData = await _distributedCache.GetStringAsync(key);
            return jsonData ?? default;
        }
    }
}
