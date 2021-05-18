using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace Library.API.Filters
{
    public class RequestRateLimitingMiddleware
    {
        private const int Limit = 10;
        private readonly RequestDelegate next;
        private readonly IMemoryCache requestStore;

        public RequestRateLimitingMiddleware(RequestDelegate next, IMemoryCache requestStore)
        {
            this.next = next;
            this.requestStore = requestStore;
        }

        /// <summary>
        /// 限制每分钟内使用同一方法对同一个资源仅能发起10次请求
        /// 可使用第三方库AspNetCoreRateLimit
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var requestKey = $"{context.Request.Method}-{context.Request.Path}";
            var cacheOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(1)
            };

            if (requestStore.TryGetValue(requestKey, out int hitCount))
            {
                if (hitCount < Limit)
                {
                    await ProcessRequest(context, requestKey, hitCount, cacheOptions);
                }
                else
                {
                    //  X-RateLimit-RetryAfter：超出限制后能够再次正常访问的时间。
                    context.Response.Headers["X-RateLimit-RetryAfter"] = cacheOptions.AbsoluteExpiration?.ToString();
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                }
            }
            else
            {
                await ProcessRequest(context, requestKey, hitCount, cacheOptions);
            }
        }

        private async Task ProcessRequest(HttpContext context, string requestKey, int hitCount, MemoryCacheEntryOptions cacheOptions)
        {
            hitCount++;
            requestStore.Set(requestKey, hitCount, cacheOptions);
            //  X-RateLimit-Limit：同一个时间段所允许的请求的最大数目
            context.Response.Headers["X-RateLimit-Limit"] = Limit.ToString();
            //  X-RateLimit-Remaining：在当前时间段内剩余的请求的数量。
            context.Response.Headers["X-RateLimit-Remaining"] = (Limit - hitCount).ToString();
            await next(context);
        }
    }
}
