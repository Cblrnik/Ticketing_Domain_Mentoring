using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Ticketing.Caching.Services;

namespace Ticketing.Caching
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly uint _timeToLiveSeconds;

        public CachedAttribute(uint timeToLiveSeconds = 120)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheKey = KeyCacheGenerator.GenerateKey(context.HttpContext.Request);
            var cacheService = ServiceProviderServiceExtensions.GetRequiredService(
                context.HttpContext.RequestServices,
                typeof(IResponseCacheService)) as IResponseCacheService; ;
            var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200,
                };

                context.Result = contentResult;
                return;
            }

            var executedContext = await next();

            if (executedContext.Result is OkObjectResult ok)
            {
                await cacheService.CacheResponseAsync(cacheKey, ok.Value, _timeToLiveSeconds);
            }
        }
    }
}
