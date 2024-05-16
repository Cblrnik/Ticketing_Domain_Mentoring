using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ticketing.Caching.Services
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string key, object? value, uint timeToLiveInSeconds);

        Task<string> GetCachedResponseAsync(string key);

        Task DeleteByKey(string key);
    }
}
