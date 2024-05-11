using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.Caching.Services
{
    public class BusinessRedisCacheService : IResponseCacheService
    {
        private readonly IResponseCacheService _cache;

        public BusinessRedisCacheService(IResponseCacheService cache)
        {
            _cache = cache;
        }

        public void DeleteByKeys(string pathAndQuery)
        {
            _cache.DeleteByKeys(pathAndQuery);
        }

        public async Task CacheResponseAsync(string key, object value, uint timeToLiveInSeconds)
        {
            await _cache.CacheResponseAsync(key, value, timeToLiveInSeconds);
        }

        public async Task<string> GetCachedResponseAsync(string key)
        {
            return await _cache.GetCachedResponseAsync(key);
        }
    }
}
