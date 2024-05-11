using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Ticketing.Caching.Services
{
    public class RedisCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _distributedCache;

        public RedisCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task CacheResponseAsync(string key, object value, uint timeToLiveInSeconds = 120)
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

        public void DeleteByKeys(string pathAndQuery)
        {
            var cacheSettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("RedisCacheSettings");
            using var redis = ConnectionMultiplexer.Connect($"{cacheSettings["host"]}:{cacheSettings["port"]},allowAdmin=true");
            IDatabase db = redis.GetDatabase();

            var keys = redis.GetServer(cacheSettings["host"], int.Parse(cacheSettings["port"])).Keys();

            var (path, id) = this.GetControllerAndId(pathAndQuery);
            var personalKeyToDelete = KeyCacheGenerator.GenerateKey(path + "find", $"?id={id}");
            var keysToDelete = keys.Select(key => (string)key)
                                   .Where(x => x.Contains(path + "get") || x.Contains(personalKeyToDelete));

            foreach (var item in keysToDelete)
            {
                db.KeyDelete(item);
            }
        }

        public async Task<string> GetCachedResponseAsync(string key)
        {
            var jsonData = await _distributedCache.GetStringAsync(key);
            return jsonData ?? default;
        }

        private (string path, string id) GetControllerAndId(string path)
        {
            var stringArray = path.Split('/');
            var controllerName = new StringBuilder(path).Replace(stringArray[^1], string.Empty);
            var id = stringArray[^1].Split('|')[^1];
            return (controllerName.ToString(), id);
        }
    }
}
