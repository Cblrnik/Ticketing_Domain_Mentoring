using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Ticketing.Caching.Services;

namespace Ticketing.Caching
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheCanChangedAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next();
            var cacheService = ServiceProviderServiceExtensions.GetRequiredService(
                context.HttpContext.RequestServices,
                typeof(IResponseCacheService)) as IResponseCacheService;
            if (executedContext.Result is OkObjectResult)
            {
                var cacheKey = KeyCacheGenerator.GenerateKey(context.HttpContext.Request);
                cacheService?.DeleteByKey(cacheKey);
            }
        }
    }
}